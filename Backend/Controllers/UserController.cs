using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;
using Backend.ExternalApiClients.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserExapiClient _userExapiClient;
    private readonly IUserService _userService;
    private readonly IAddressService _addressService;

    public UserController(IUserExapiClient userExapiClient, IUserService userService, IAddressService addressService)
    {
        _userExapiClient = userExapiClient;
        _userService = userService;
        _addressService = addressService;
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredUsers([FromQuery] string? gender, [FromQuery] string? email, [FromQuery] string? username)
    {
        //Se la query di ricerca si aspetta piú risultati non possiamo risparmiarci la chiamata all'EXAPI, dato che corriamo il rischio di non mostrare tutti i dati
        //Mentre invece se la query si aspetta un risultato preciso possiamo fare una ricerca nel DB locale per vedere se é giá presente e tentare di risparmiare la chiamata all'EXAPI
        //Questo é possibile con EMAIL e USERNAME che sono parametri univoci
        bool isSingleResult = false;

        var dbUsers = await _userService.GetAllUsersWithAddressesAsync();

        //Costruiamo la query per filtrare i dati presenti nel DB
        IQueryable<User> query = dbUsers.AsQueryable();

        //La chiamata all'endpoint dará in output un singolo risultato solo quando nella query di filtraggio é presente l'email o la password
        //mentre invece se si filtra per gender o si invia la chiamata senza filtri questa dará in output piú risultati e quindi isSingleResult rimarrá 'false'
        if (!String.IsNullOrEmpty(gender))
            query = query.Where(u => u.Gender.Equals(gender));
        if (!String.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email.Equals(email));
            isSingleResult = true;
        }
        if (!String.IsNullOrEmpty(username))
        {
            query = query.Where(u => u.Username.Equals(username));
            isSingleResult = true;
        }

        if (isSingleResult)
        {
            //Esegue la query e filtra i dati del DB
            List<User> users = query.ToList();
            //Se la query torna una lista non vuota significa che ha trovato il dato che cerchiamo
            if (!users.IsNullOrEmpty())
            {
                return Ok(_userService.DbToMyApiDTO(users[0]));
            }
            else
            {
                //Altrimenti (se l'utente non é presente nel DB dobbiamo recuperarlo dall'EXAPI)
                var exapiUsers = _userExapiClient.GetPaginatedUsers(10).Result;

                if (exapiUsers is null)
                {
                    return NotFound();
                }

                //Costruiamo la query di filtraggio per la lista di dati che ci arriva dall'EXAPI
                IQueryable<ExapiUserDTO> queryForExapiList = exapiUsers.AsQueryable();

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
                    return NotFound();
                }

                //Salviamo l'indirizzo e l'utente nel nostro db locale
                var addressId = await _addressService.CreateAddressAsync(filteredExapiUser.Address);
                await _userService.CreateUserAsync(filteredExapiUser, addressId);

                //Mappiamo da exapi a myApi per mostrato il risultato nel form che ci é stato chiesto
                var mappedMyApiUser = _userService.ExapiToMyApiDTO(filteredExapiUser);
                mappedMyApiUser.AddressId = addressId.ToString();
                return Ok(mappedMyApiUser);
            }
        }
        else
        {
            //Altrimenti (se non si tratta di un risultato singolo)

            var exapiUsers = _userExapiClient.GetPaginatedUsers(10).Result;

            //Se non riceviamo dati ritorna not found
            if (exapiUsers.IsNullOrEmpty() || exapiUsers is null)
            {
                return NotFound();
            }

            List<MyApiUserDTO> mappedMyApiUsers = [];

            foreach (var exapiUser in exapiUsers)
            {
                //Prima di procedere con il salvataggio controlla che questo utente non esista giá nel DB locale
                if (dbUsers.Any(u => u.Id == exapiUser.Id))
                {
                    continue;
                }

                //Lo salva nel DB
                int addressId = await _addressService.CreateAddressAsync(exapiUser.Address);
                await _userService.CreateUserAsync(exapiUser, addressId);

                //Converte da exapiUser in myApiUser
                var mappedMyApiUser = _userService.ExapiToMyApiDTO(exapiUser);
                mappedMyApiUsers.Add(mappedMyApiUser);
                mappedMyApiUser.AddressId = addressId.ToString();
            }

            //L'unico filtro che possiamo aspettarci é quello del gender, quindi controliamo se é presente ed eventualmente lo eseguiamo
            if (!String.IsNullOrEmpty(gender))
            {
                return Ok(mappedMyApiUsers.Where(u => u.Gender.Equals(gender)));
            }

            //Ritorna gli usenti
            return Ok(mappedMyApiUsers);
        }
    }
}

