using System;
using WaadManager.Common.Models;

namespace WaadManager.Common.models
{
    public class DeltaAction : Tuple<DtDelta, DtDelta>
    {
        public DtDelta OldDelta { get { return Item1; } }
        public DtDelta NewDelta { get { return Item2; } }

        public DeltaAction(DtDelta item1, DtDelta item2) : base(item1, item2) { }
    }
}
