//using Microsoft.Azure.Cosmos.Table;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Models
//{
//    public interface IThunderBallRepository
//    {
//        Task<List<ThunderBallDto>> GetById(string drawNumber);
//        Task<List<ThunderBallDto>> ListAll();
//        Task<TableResult> Save(ThunderBallDto tball);
//        //Task<bool> SaveAll(List<ThunderBallDto> tballs);
//        Task<TableResult> Delete(ThunderBallDto tball);
//        Task<TableBatchResult[]> DeleteAll();

//        Task<List<ThunderBallDto>> ListUpTo(string v);
//    }
//}
