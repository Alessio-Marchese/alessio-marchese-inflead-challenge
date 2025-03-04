using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;
using Backend.ExternalApiClients.Interfaces;
using Backend.Mappers.Interfaces;
using Backend.Repository.Interfaces;
using Backend.Services.Interfaces;
using Backend.Utility;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _userMapper;
    private readonly IUserExapiClient _userExapiClient;
    private readonly IAddressService _addressService;
    public UserService(IUserRepository userRepository, IUserMapper userMapper, IUserExapiClient userExapiClient, IAddressService addressService)
    {
        _addressService = addressService;
        _userExapiClient = userExapiClient;
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<List<User>> GetAllUsersWithAddressesAsync()
    {
        return await _userRepository.GetAllUsersWithAddressesAsync();
    }

    public bool CheckIfIsSingleResult(string? email, string? username)
    {
        bool isSingleResult = false;

        if (!String.IsNullOrEmpty(email) || !String.IsNullOrEmpty(username))
        {
            isSingleResult = true;
        }
            
        return isSingleResult;
    }

    public async Task<Result<MyApiUserDTO>> FindSingleUser(string? gender, string? email, string? username, List<User> dbUsers)
    {
        //Costruiamo la query per filtrare i dati presenti nel DB
        IQueryable<User> query = dbUsers.AsQueryable();

        //La chiamata all'endpoint dará in output un singolo risultato solo quando nella query di filtraggio é presente l'email o la password
        //mentre invece se si filtra per gender o si invia la chiamata senza filtri questa dará in output piú risultati e quindi isSingleResult rimarrá 'false'
        if (!String.IsNullOrEmpty(gender))
            query = query.Where(u => u.Gender.Equals(gender));
        if (!String.IsNullOrEmpty(email))
            query = query.Where(u => u.Email.Equals(email));
        if (!String.IsNullOrEmpty(username))
            query = query.Where(u => u.Username.Equals(username));

        //Esegue la query e filtra i dati del DB
        List<User> users = query.ToList();
            //Se la query torna una lista non vuota significa che ha trovato il dato che cerchiamo
            if (!users.IsNullOrEmpty())
            {
                return Result<MyApiUserDTO>.Success(_userMapper.DbToMyApiDTO(users[0]));
            }
            else
            {
                //Altrimenti (se l'utente non é presente nel DB dobbiamo recuperarlo dall'EXAPI)
                var exapiUsers = _userExapiClient.GetPaginatedUsers(10).Result;

                if (!exapiUsers.IsOk)
                {
                    return Result<MyApiUserDTO>.Failure(exapiUsers.ErrorMessage);
                }

                //Costruiamo la query di filtraggio per la lista di dati che ci arriva dall'EXAPI
                IQueryable<ExapiUserDTO> queryForExapiList = exapiUsers.Data.AsQueryable();

                //Questa volta filtriamo solo email e password o eventualmente entrambi. Possiamo dare per scontato  
                //che ci possono essere solo questi due filtri dato che ci troviamo nell'if del risultato singolo
                if (!String.IsNullOrEmpty(email))
                    queryForExapiList = queryForExapiList.Where(u => u.Email.Equals(email));
                if (!String.IsNullOrEmpty(username))
                    queryForExapiList = queryForExapiList.Where(u => u.Username.Equals(username));

                //Prima di lavorare sul risultato controlliamo se questo é null o vuoto.
                //L'EXAPI ci da 10 utenti random, tra questi potrebbe non esserci il dato che cerchiamo
                var filteredExapiUser = queryForExapiList.ToList().IsNullOrEmpty() ? null : queryForExapiList.ToList()[0];

                if (filteredExapiUser is null)
                {
                    return Result<MyApiUserDTO>.Failure("L'utente che stai cercando con i filtri specificati non é presente nell'API esterna");
                }

                //Salviamo l'indirizzo e l'utente nel nostro db locale
                var addressId = await _addressService.CreateAddressAsync(filteredExapiUser.Address);
                filteredExapiUser.AddressId = addressId;
                await _userRepository.CreateUserAsync(_userMapper.ExapiToDb(filteredExapiUser));

                //Mappiamo da exapi a myApi per mostrato il risultato nel form che ci é stato chiesto
                var mappedMyApiUser = _userMapper.ExapiToMyApiDTO(filteredExapiUser);
                mappedMyApiUser.AddressId = addressId.ToString();
                return Result<MyApiUserDTO>.Success(mappedMyApiUser);
            }
    }

    public async Task<Result<List<MyApiUserDTO>>> FindManyUsers(string? gender, string? email, string? username, List<User> dbUsers)
    {
        //Altrimenti (se non si tratta di un risultato singolo)
        var resultExapiUsers = _userExapiClient.GetPaginatedUsers(10).Result;

        //Se non riceviamo dati ritorna not found
        if (!resultExapiUsers.IsOk)
        {
            return Result<List<MyApiUserDTO>>.Failure(resultExapiUsers.ErrorMessage);
        }

        List<MyApiUserDTO> mappedMyApiUsers = [];

        foreach (var exapiUser in resultExapiUsers.Data)
        {
            //Prima di procedere con il salvataggio controlla che questo utente non esista giá nel DB locale
            if (dbUsers.Any(u => u.Id == exapiUser.Id))
            {
                continue;
            }

            //Lo salva nel DB
            int addressId = await _addressService.CreateAddressAsync(exapiUser.Address);
            exapiUser.AddressId = addressId;
            await _userRepository.CreateUserAsync(_userMapper.ExapiToDb(exapiUser));

            //Converte da exapiUser in myApiUser
            var mappedMyApiUser = _userMapper.ExapiToMyApiDTO(exapiUser);
            mappedMyApiUsers.Add(mappedMyApiUser);
            mappedMyApiUser.AddressId = addressId.ToString();
        }

        //L'unico filtro che possiamo aspettarci é quello del gender, quindi controliamo se é presente ed eventualmente lo eseguiamo
        if (!String.IsNullOrEmpty(gender))
        {
            return Result<List<MyApiUserDTO>>.Success(mappedMyApiUsers.Where(u => u.Gender.Equals(gender)).ToList());
        }

        //Ritorna gli usenti
        return Result<List<MyApiUserDTO>>.Success(mappedMyApiUsers);
    }
}
