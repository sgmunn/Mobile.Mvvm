// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupExtensions.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Mvvm.ViewModel.Dialog
{
    using System;

    public static class GroupExtensions
    {
        public static int ViewModelCount(this IGroup group)
        {
            return group.Rows.Count + (group.Header != null ? 1 : 0) + (group.Footer != null ? 1 : 0);
        }

        /// <summary>
        /// Gets the view model for the given index, taking into account Heade and Footer
        /// </summary>
        public static IViewModel ViewModelAtIndex(this IGroup group, int index)
        {
            if (index == 0 && group.Header != null)
            {
                return group.Header;
            }

            // reduce the index by one if there is a header, row 0 is index 1 in this case
            var rowIndex = index + (group.Header != null ? -1 : 0);
            if (rowIndex >= group.Rows.Count)
            {
                // assume footer for anything past the number of rows
                if (group.Footer != null)
                {
                    return group.Footer;
                }

                throw new ArgumentOutOfRangeException("index");
            }

            return group.Rows[rowIndex];
        }

        public static bool IsHeaderOrFooterAtIndex(this IGroup group, int index)
        {
            if (index == 0)
            {
                if (group.Header != null)
                {
                    return true;
                }

                if (group.Rows.Count == 0 && group.Footer != null)
                {
                    return true;
                }
            }

            var rowIndex = index + (group.Header != null ? -1 : 0);
            if (rowIndex >= group.Rows.Count)
            {
                // assume footer for anything past the number of rows
                if (group.Footer != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
