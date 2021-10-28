using DimensionService.Models.DimensionModels;

namespace DimensionService.Dao.CallRecord
{
    public interface ICallRecordDAO
    {
        bool AddCallRecord(CallRecordModel callRecord);

        bool EffectiveCallRecord(string userID);
    }
}
