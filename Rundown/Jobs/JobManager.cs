using Il2CppInterop.Runtime;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace CustomExpeditionEvents.Rundown.Jobs
{
    internal static class JobManager
    {
        public static LG_FactoryJob CreateJob<T>()
            where T :
#if IL2CPP_INHERITANCE
            LG_FactoryJob
#else
            ICustomJob
#endif
            
            , new()
        {
            return CreateJob<T>(() => new T());
        }

        public static LG_FactoryJob CreateJob<T>(Func<T> creator)
            where T :
#if IL2CPP_INHERITANCE
            LG_FactoryJob
#else
            ICustomJob
#endif
        {
#if IL2CPP_INHERITANCE
            return creator.Invoke();
#else
            LG_FactoryJob job = new LG_FactoryJob();
            s_jobInstances.Add(job);
            s_jobs[job.Pointer] = creator.Invoke();
            return job;
#endif
        }

#if !IL2CPP_INHERITANCE
        private static readonly Dictionary<IntPtr, ICustomJob> s_jobs = new();
        private static readonly List<LG_FactoryJob> s_jobInstances = new();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Cleanup()
        {
            s_jobs.Clear();
            s_jobInstances.Clear();
        }



        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool? InvokeBuild(LG_FactoryJob job)
        {
            if (s_jobs.TryGetValue(job.Pointer, out ICustomJob? customJob))
            {
                return customJob.Build();
            }
            return null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string? InvokeGetName(LG_FactoryJob job)
        {
            if (s_jobs.TryGetValue(job.Pointer, out ICustomJob? customJob))
            {
                return customJob.GetName();
            }
            return null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool? InvokeTakeFullFrame(LG_FactoryJob job)
        {
            if (s_jobs.TryGetValue(job.Pointer, out ICustomJob? customJob))
            {
                return customJob.TakeFullFrame();
            }
            return null;
        }

#endif
    }
}
