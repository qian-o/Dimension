using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using System.Linq;

namespace DimensionService.Dao.CallRecord
{
    public class CallRecordDAO : ICallRecordDAO
    {
        public bool AddCallRecord(CallRecordModel callRecord)
        {
            using DimensionContext context = new();
            context.CallRecord.Add(callRecord);
            return context.SaveChanges() > 0;
        }

        public bool EffectiveCallRecord(string userID)
        {
            using DimensionContext context = new();
            return context.CallRecord.FirstOrDefault(item => item.UserID == userID && item.Effective) != null;
        }
    }
}
