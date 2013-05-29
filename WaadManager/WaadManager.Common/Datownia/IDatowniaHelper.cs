using System.Collections.Generic;
using WaadManager.Common.Models;

namespace WaadManager.Common.Datownia
{
    public interface IDatowniaHelper
    {
        IEnumerable<DtDelta> GetDeltas(string documentName, int currentDelta);
        IEnumerable<DtUser> GetAllUsers(out int sequence);
        IEnumerable<DtEvent> GetAllEvents(out int sequence);
        IEnumerable<DtGroup> GetAllGroups(out int sequence);
        IEnumerable<DtUserGroup> GetAllUserGroups(out int sequence);
    }
}
