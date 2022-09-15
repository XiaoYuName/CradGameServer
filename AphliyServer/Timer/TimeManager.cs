using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using AphliyServer.Concurrent;

namespace AphliyServer.Timera
{
    /// <summary>
    /// 定时任务(计时器)管理类
    /// </summary>
    public class TimeManager
    {
        private static TimeManager instance = null;
        public static TimeManager Instance {
            get
            {
                lock (instance)
                {
                    if (instance == null)
                        instance = new TimeManager();
                    return instance;
                    
                }
            }
        }

        /// <summary>
        /// 这个字典存储: 任务ID:  和任务模型的映射
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModeDic = new ConcurrentDictionary<int, TimerModel>();
        /// <summary>
        /// 要移除的任务ID列表
        /// </summary>
        private List<int> removeList = new List<int>();

        /// <summary>
        /// 用来表示ID : 每次都会累加
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);
        /// <summary>
        /// 实现定时器的主要功能
        /// </summary>
        private Timer timer;

        public TimeManager()
        {
            timer = new Timer(10);
            timer.Elapsed += TimerOnElapsed;
        }

        /// <summary>
        /// 达到时间间隔时候触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            lock (removeList)
            {
                TimerModel tempModel = null;
                foreach (var keyID in removeList)
                {
                    idModeDic.TryRemove(keyID, out tempModel);
                }
                removeList.Clear();
            }
            
            foreach (var model in idModeDic.Values)
            {
                if (model.Timer <= DateTime.Now.Ticks)
                    model.Run();
            }
        }


        /// <summary>
        /// 添加定时任务: 指定触发的时间
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="timerDelegate">委托</param>
        public void AddTimerEvent(DateTime dateTime,TimerDelegate timerDelegate)
        {
           long delayTime = dateTime.Ticks - DateTime.Now.Ticks;
           if (delayTime <= 0) return;
           AddTimerEvent(delayTime,timerDelegate);
        }

        /// <summary>
        /// 添加定时任务 : 指定延迟的时间
        /// </summary>
        /// <param name="delayTime">延迟时间,单位是毫秒</param>
        /// <param name="timerDelegate"></param>
        public void AddTimerEvent(long delayTime, TimerDelegate timerDelegate)
        {
            TimerModel model = new TimerModel(id.Add_Get(),DateTime.Now.Ticks + delayTime,timerDelegate);
            idModeDic.TryAdd(model.id, model);
        }
    }
}