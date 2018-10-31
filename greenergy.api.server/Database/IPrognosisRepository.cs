using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Models;

namespace Greenergy.Database
{
    public interface IPrognosisRepository
    {
        Task UpdatePrognosisData(List<PrognosisData> prognoses);
        Task<List<PrognosisData>> PrognosisMinimum();
    }
}