namespace IPriziv.Server.Engine.Recruits
{
    /// <summary>Правила значения НПУ у призывника (6–10 или пусто).</summary>
    public static class RecruitNpuRules
    {
        public const byte Min = 6;
        public const byte Max = 10;

        public static bool IsValid(byte? npu)
            => npu is null || (npu.Value >= Min && npu.Value <= Max);

        public static string InvalidMessage
            => $"НПУ должно быть числом от {Min} до {Max} либо пустым";
    }
}
