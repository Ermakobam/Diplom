using System.Diagnostics;

namespace IPriziv.Server.Engine.Utils
{
    public class Diagnostic
    {
        public (int average, int deviation)[] Test(int count, params Action[] actions)
        {
            var results = new (int average, int deviation)[actions.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var actionResults = new int[count];
                for (int j = 0; j < count; j++)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    actions[i].Invoke();
                    stopwatch.Stop();
                    actionResults[j] = (int)stopwatch.ElapsedMilliseconds;
                }
                var average = actionResults.Sum() / count;
                results[i] = (average, actionResults.Sum(x => Math.Abs(x - average)) / count);
            }
            return results;
        }
        public async Task<(int average, int deviation)[]> TestAsync(int count, params Func<Task>[] actions)
        {
            var results = new (int average, int deviation)[actions.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var actionResults = new int[count];
                for (int j = 0; j < count; j++)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await actions[i].Invoke();
                    stopwatch.Stop();
                    actionResults[j] = (int)stopwatch.ElapsedMilliseconds;
                }
                var average = actionResults.Sum() / count;
                results[i] = (average, actionResults.Sum(x => Math.Abs(x - average)) / count);
            }
            return results;
        }
    }
}
