namespace IPriziv.Server.Data.Constants
{
    public static class RecruitStates
    {
        public static readonly byte Delivered = 1;
        public static readonly byte UnControled = 2;
        public static readonly byte Controled = 3;
        public static readonly byte Complected = 9;
        public static readonly byte Sent = 10;
        public static readonly byte ReturnedByMMC = 15;
        public static readonly byte ReturnedByFamilyReason = 16;
        public static readonly byte ReturnedByOtherReason = 17;
        public static readonly byte DeliverDeclared = 18;
        public static readonly byte[] Returned = new[] { ReturnedByFamilyReason, ReturnedByMMC, ReturnedByOtherReason };
        public static readonly byte[] SelectionPassed = new[] { Controled, UnControled };
    }
}
