//The MIT License (MIT)
//Copyright (c) Microsoft Corporation.  All rights reserved.

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

namespace System.ComponentModel
{
    using System;
    using System.Diagnostics;
    using System.Security.Permissions;

    /// <devdoc>
    /// <para>Provides data for the <see langword='ErrorsChanged'/>
    /// event.</para>
    /// </devdoc>
    public class DataErrorsChangedEventArgs : EventArgs
    {
        private readonly string propertyName;

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DataErrorsChangedEventArgs'/>
        /// class.</para>
        /// </devdoc>
        public DataErrorsChangedEventArgs(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <devdoc>
        ///    <para>Indicates the name of the property whose errors changed.</para>
        /// </devdoc>
        public virtual string PropertyName
        {
            get
            {
                return propertyName;
            }
        }
    }
}