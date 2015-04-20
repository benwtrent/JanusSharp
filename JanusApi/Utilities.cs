#region License
/** Janus Sharp
 * Copyright (C) <2014> Benjamin Trent <ben.w.trent@gmail.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Library Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JanusApi
{
  public class MTSafeRefCounter
  {
    private int ref_count;
    private static readonly object ref_lock = new object();
    private bool allowed_ref;
    private static readonly object allowed_ref_lock = new object();


    /// <summary>
    /// A very simple references counter that allows a block to be made so that additional references cannot be made.
    /// Initial reference count is 0 and references are allowed
    /// </summary>
    public MTSafeRefCounter()
    {
      ref_count = 0;
      allowed_ref = true;
    }

    /// <summary>
    /// Returns the current reference count
    /// </summary>
    public int ReferenceCount
    {
      get
      {
        lock (ref_lock)
        {
          return ref_count;
        }
      }
    }

    /// <summary>
    /// Increase the reference counter
    /// </summary>
    /// <returns>Returns True if not blocked and could add references, False otherwise</returns>
    public bool IncRef()
    {
      lock (allowed_ref_lock)
      {
        if (allowed_ref)
        {
          lock (ref_lock)
          {
            ref_count++;
            Console.WriteLine("RefCount = " + ref_count + " thread: " + Thread.CurrentThread.ManagedThreadId.ToString());
            
            return true;
          }
        }
        return false;
      }
    }

    /// <summary>
    /// Decreases the reference if it is greater than 0
    /// </summary>
    public void DecRef()
    {
      lock (ref_lock)
      {
        Console.WriteLine("Dec RefCount = " + ref_count + " thread: " + Thread.CurrentThread.ManagedThreadId.ToString());
        if (ref_count > 0)
          ref_count--;
      }
    }

    /// <summary>
    /// Disallows any more references to be made
    /// </summary>
    public void BlockIncrease()
    {
      lock (allowed_ref_lock)
      {
        allowed_ref = false;
      }
    }

    /// <summary>
    /// Allows references to be made
    /// </summary>
    public void UnblockIncrease()
    {
      lock (allowed_ref_lock)
      {
        allowed_ref = true;
      }
    }
  }

  public class DynamicDelayExecute
  {
    private Thread count_down_thread;

    /// <summary>
    /// The event handler to fire when the delay time is reached.
    /// </summary>
    public event EventHandler OnDelayExhausted;
    private uint count;
    private static readonly object count_lock_obj = new object();

    /// <summary>
    /// Allows delay execution for a given number of seconds. Will not start the process of delaying until Start() is called
    /// </summary>
    /// <param name="delay">The number of seconds to delay</param>
    public DynamicDelayExecute(uint delay)
    {
      count = delay;
      count_down_thread = new Thread(new ThreadStart(DelayThread));
    }

    /// <summary>
    /// Starts the countdown towards the delayed execution
    /// </summary>
    public void Start()
    {
      if (count_down_thread.ThreadState == ThreadState.Stopped)
      {
        count_down_thread = new Thread(new ThreadStart(DelayThread));
        count_down_thread.Start();
      }
      else
        count_down_thread.Start();
    }

    private void DelayThread()
    {
      while (true)
      {
        lock (count_lock_obj)
        {
          if (count == 0)
            break;
          Monitor.Wait(count_lock_obj, TimeSpan.FromSeconds(1));
          if (count == 0)
            break;
          count--;
        }
      }
      OnDelayLimitReached();
      lock (count_lock_obj)
      {
        Monitor.PulseAll(count_lock_obj);
      }
    }

    private void OnDelayLimitReached()
    {
      if (OnDelayExhausted != null)
      {
        OnDelayExhausted(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Calls ResetDelay and sets count to 0.
    /// Signals the event to occur in the delay thread and blocks until the thread is finished
    /// </summary>
    public void Immediate()
    {
      lock (count_lock_obj)
      {
        count = 0;
        Monitor.PulseAll(count_lock_obj);
        if(count_down_thread.IsAlive)
          Monitor.Wait(count_lock_obj);
      }
    }

    /// <summary>
    /// Resets the Delay execution to the passed delay object.
    /// If the event has already occured, then it will not be fired again.
    /// To reset the entire delay(for even after execution) call HardReset(uint delay);
    /// </summary>
    /// <param name="delay">The new delay time</param>
    public void ResetDelay(uint delay)
    {
      lock (count_lock_obj)
      {
        count = delay;
      }
    }

    /// <summary>
    /// Executes the given event if it has not been executed, then starts new countdown given the delay.
    /// </summary>
    /// <param name="delay">New delay time</param>
    public void HardReset(uint delay)
    {
      lock (count_lock_obj)
      {
        count = 0;
        Monitor.PulseAll(count_lock_obj);
        if(count_down_thread.IsAlive)
          Monitor.Wait(count_lock_obj);
        count = delay;
        Start();
      }
    }
  }
}
