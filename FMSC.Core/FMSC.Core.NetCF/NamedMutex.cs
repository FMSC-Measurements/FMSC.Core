//The MIT License (MIT)
//Copyright (c) 2001-2014 Tacke Consulting, LLC

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenNETCF.Threading
{
    /// <summary>
    /// A synchronization primitive than can also be used for interprocess synchronization.
    /// </summary>
    public class NamedMutex : WaitHandle
    {
        private bool createdNew;

        /// <summary>
        /// Initializes a new instance of the Mutex2 class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex, a string that is the name of the mutex, and a Boolean value that, when the method returns, will indicate whether the calling thread was granted initial ownership of the mutex.
        /// </summary>
        /// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
        /// <param name="name">The name of the Mutex.
        /// If the value is a null reference (Nothing in Visual Basic), the Mutex is unnamed.</param>
        /// <param name="createdNew">When this method returns, contains a Boolean that is true if the calling thread was granted initial ownership of the mutex; otherwise, false.
        /// This parameter is passed uninitialized.</param>
        public NamedMutex(bool initiallyOwned, string name, out bool createdNew)
            : this(initiallyOwned, name)
        {
            createdNew = this.createdNew;
        }

        /// <summary>
        /// Initializes a new instance of the Mutex2 class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex, and a string that is the name of the mutex.
        /// </summary>
        /// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
        /// <param name="name">The name of the Mutex.
        /// If the value is a null reference (Nothing in Visual Basic), the Mutex is unnamed.</param>
        /// <exception cref="ApplicationException">Failed to create mutex.</exception>
        public NamedMutex(bool initiallyOwned, string name)
        {
            IntPtr hMutex = CreateMutex(IntPtr.Zero, initiallyOwned, name);
            if (hMutex == IntPtr.Zero)
            {
                throw new ApplicationException("Failure creating mutex: " + Marshal.GetLastWin32Error().ToString("X"));
            }
            if (Marshal.GetLastWin32Error() == ERROR_ALREADY_EXISTS)
            {
                createdNew = false;
            }
            else
            {
                createdNew = true;
            }

            this.Handle = hMutex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedMutex"/> class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex.
        /// </summary>
        /// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
        public NamedMutex(bool initiallyOwned) : this(initiallyOwned, null) { }

        /// <summary>
        /// Initializes a new instance of the Mutex2 class with default properties.
        /// </summary>
        public NamedMutex() : this(false, null) { }

        #region OpenExisting

        /// <summary>
        /// Opens an existing named mutex.
        /// </summary>
        /// <param name="name">The name of a system-wide named mutex object.</param>
        /// <returns>A <see cref="NamedMutex"/> object that represents a named system mutex.</returns>
        /// <remarks>The OpenExisting method attempts to open an existing named mutex.
        /// If the system mutex does not exist, this method throws an exception instead of creating the system object.
        /// Two calls to this method with the same value for name do not necessarily return the same <see cref="NamedMutex"/> object, even though they represent the same named system mutex.</remarks>
        /// <exception cref="ArgumentException">name is a zero-length string.
        /// -or-
        /// name is longer than 260 characters.</exception>
        /// <exception cref="ArgumentNullException">name is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="WaitHandleCannotBeOpenedException">The named mutex does not exist.</exception>
        public static NamedMutex OpenExisting(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length < 1)
            {
                throw new ArgumentException("name is a zero-length string.");
            }
            if (name.Length > 260)
            {
                throw new ArgumentException("name is longer than 260 characters.");
            }

            bool created;
            NamedMutex m = new NamedMutex(false, name, out created);
            if (created)
            {
                //mutex doesnt already exist
                m.Dispose(true);
                throw new WaitHandleCannotBeOpenedException();
            }
            return m;
        }

        #endregion OpenExisting

        /// <summary>
        /// Releases the <see cref="NamedMutex"/> once.
        /// </summary>
        /// <exception cref="ApplicationException">The calling thread does not own the mutex.</exception>
        public void ReleaseMutex()
        {
            if (!ReleaseMutex(this.Handle))
            {
                throw new ApplicationException("The calling thread does not own the mutex.");
            }
        }

        /// <summary>
        /// Blocks the current thread until the current <see cref="NamedMutex"/> receives a signal.
        /// </summary>
        /// <returns>true if the current instance receives a signal. if the current instance is never signaled, <see cref="WaitOne()"/> never returns.</returns>
        public override bool WaitOne()
        {
            return WaitOne(-1, false);
        }

        /// <summary>
        /// When overridden in a derived class, blocks the current thread until the current <see cref="NamedMutex"/> receives a signal, using 32-bit signed integer to measure the time interval and specifying whether to exit the synchronization domain before the wait.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Threading.Timeout.Infinite (-1) to wait indefinitely.</param>
        /// <param name="notApplicableOnCE">Just pass false</param>
        /// <returns>true if the current instance receives a signal; otherwise, false.</returns>
        public override bool WaitOne(Int32 millisecondsTimeout, bool notApplicableOnCE)
        {
            return (WaitForSingleObject(this.Handle, millisecondsTimeout) != WAIT_TIMEOUT);
        }

        /// <summary>
        /// When overridden in a derived class, blocks the current thread until the current instance receives a signal, using a TimeSpan to measure the time interval and specifying whether to exit the synchronization domain before the wait.
        /// </summary>
        /// <param name="aTs">A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="notApplicableOnCE">Just pass false</param>
        /// <returns>true if the current instance receives a signal; otherwise, false.</returns>
        public bool WaitOne(TimeSpan aTs, bool notApplicableOnCE)
        {
            return (WaitForSingleObject(this.Handle, (int)aTs.TotalMilliseconds) != WAIT_TIMEOUT);
        }

        /// <summary>
        /// Releases all resources held by the current <see cref="WaitHandle"/>
        /// </summary>
        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="explicitDisposing"></param>
        protected override void Dispose(bool explicitDisposing)
        {
            if (this.Handle != InvalidHandle)
            {
                CloseHandle(this.Handle);
            }
            this.Handle = InvalidHandle;
            base.Dispose(explicitDisposing);
        }

        private const Int32 ERROR_ALREADY_EXISTS = 183;
        public const Int32 WAIT_TIMEOUT = 0x102;

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateMutex(
            IntPtr lpMutexAttributes,
            bool InitialOwner,
            string MutexName);

        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr hHandle, Int32 dwMilliseconds);

        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}

/// <summary>
/// The exception that is thrown when an attempt is made to open a system mutex or semaphore that does not exist.
/// </summary>
/// <remarks>Instances of the Mutex class and the Semaphore class can represent named system synchronization objects.
/// When you use the <see cref="NamedMutex.OpenExisting"/> method or the <see cref="Semaphore.OpenExisting"/> method to open a named system object that does not exist, a <see cref="WaitHandleCannotBeOpenedException"/> is thrown.</remarks>
public class WaitHandleCannotBeOpenedException : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WaitHandleCannotBeOpenedException"/> class with default values.
    /// </summary>
    public WaitHandleCannotBeOpenedException()
        : base("No handle of the given name exists.")
    {
        base.HResult = -2146233044;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitHandleCannotBeOpenedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    public WaitHandleCannotBeOpenedException(string message)
        : base(message)
    {
        base.HResult = -2146233044;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitHandleCannotBeOpenedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception.
    /// If the innerException parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
    public WaitHandleCannotBeOpenedException(string message, Exception innerException)
        : base(message, innerException)
    {
        base.HResult = -2146233044;
    }
}