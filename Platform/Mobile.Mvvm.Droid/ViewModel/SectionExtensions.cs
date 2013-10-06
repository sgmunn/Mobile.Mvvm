// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionExtensions.cs" company="sgmunn">
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

namespace Mobile.Mvvm.ViewModel
{
    using System;

    public static class SectionExtensions
    {
        public static int ViewModelCount(this ISection section)
        {
            return section.Rows.Count + (section.Header != null ? 1 : 0) + (section.Footer != null ? 1 : 0);
        }

        /// <summary>
        /// Gets the view model for the given index, taking into account Heade and Footer
        /// </summary>
        public static IViewModel ViewModelAtIndex(this ISection section, int index)
        {
            if (index == 0 && section.Header != null)
            {
                return section.Header;
            }

            // reduce the index by one if there is a header, row 0 is index 1 in this case
            var rowIndex = index + (section.Header != null ? -1 : 0);
            if (rowIndex >= section.Rows.Count)
            {
                // assume footer for anything past the number of rows
                if (section.Footer != null)
                {
                    return section.Footer;
                }

                throw new ArgumentOutOfRangeException("index");
            }

            return section.Rows[rowIndex];
        }
    }
}
