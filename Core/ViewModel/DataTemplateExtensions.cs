// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTemplateExtensions.cs" company="sgmunn">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    
    // section source's need these
    // additionally, view model root contexts

    //    this.RegisterTemplate("TableViewCell_ISectionRoot")
    //        .Creates((id) => new TableViewCell(UITableViewCellStyle.Default, id))
    //            .WhenInitializing<TableViewCell>((view) => view.Accessory = UITableViewCellAccessory.DisclosureIndicator)
    //            .WhenBinding<ISectionRoot, TableViewCell>((vm, view) => view.Text = vm.ToString());


    //    public DataTemplateSelector RegisterTemplate(string reuseId)
    //    {
    //        var template = new DataTemplateSelector(reuseId);
    //
    //        this.TemplateSelectors.Add(template);
    //        return template;
    //    }

    public static class DataTemplateExtensions
    {
        public static IDataTemplate GetTemplate(this object viewModel, List<IDataTemplate> templates)
        {
            if (templates == null)
            {
                return null;
            }

            IDataTemplate exactMatch = null;
            IDataTemplate semiMatch = null;

            // keep looping even if we find one, match on last one found
            foreach (var template in templates)
            {
                switch (template.CanApplyToViewModel(viewModel))
                {
                    case TemplateMatch.Exact:
                        exactMatch = template;
                        break;

                        case TemplateMatch.Assignable:
                        semiMatch = template;
                        break;
                }
            }

            return exactMatch ?? semiMatch;
        }

        public static object GetViewForViewModel(this IDataTemplate template, object viewModel, Func<object, object> getConvertView)
        {
            return GetViewForViewModel(template, viewModel, getConvertView, null);
        }
        
        public static object GetViewForViewModel(this IDataTemplate template, object viewModel, Func<object, object> getConvertView, object root)
        {
            if (template != null)
            {
                var cell = getConvertView(template.Id);
                if (cell != null && template.ViewType != null)
                {
                    if (!template.ViewType.IsAssignableFrom(cell.GetType()))
                    {
                        cell = null;
                    }
                }

                if (cell == null)
                {
                    cell = template.CreateView(root);
                }

                template.InitializeView(cell);
                template.BindViewModel(viewModel, cell);
                return cell;
            }

            return null;
        }
    }
}
