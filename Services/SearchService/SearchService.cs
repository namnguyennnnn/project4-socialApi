using DoAn4.DTOs.UserDTO;
using DoAn4.Helper;
using DoAn4.Interfaces;
using Microsoft.Extensions.Options;



namespace DoAn4.Services.SearchService
{
    public class SearchService : ISearchService
    {
        private readonly IUserRepository _userRepository;
        public SearchService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<InfoUserDTO>> Search(string keyword)
        {
            try
            {
                var listUsers = await _userRepository.GetUsersByKeyWord(keyword);
                return listUsers;
                
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        } 
    }

}

