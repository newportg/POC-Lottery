// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Lottery.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Lottery;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Lottery.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\Pages\FetchHistory.razor"
using Lottery.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Components.RouteAttribute("/fetchhistory")]
    public partial class FetchHistory : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 82 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\Pages\FetchHistory.razor"
 
    IList<ThunderBall> items = new List<ThunderBall>();
    private bool IsSortedAscending;
    private string CurrentSortColumn;

    protected override async Task OnInitializedAsync()
    {
        items = await ThunderBallService.GetThunderballAsync();
    }

    private string GetSortStyle(string columnName)
    {
        if(CurrentSortColumn != columnName)
        {
            return string.Empty;
        }
        if(IsSortedAscending)
        {
            return "fa-sort-up";
        }
        else
        {
            return "fa-sort-down";
        }
    }

    private void SortTable(string columnName)
    {
        //Sorting against a column that is not currently sorted against.
        if (columnName != CurrentSortColumn)
        {
            //We need to force order by ascending on the new column
            //This line uses reflection and will probably perform inefficiently in a production environment.
            items = items.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            CurrentSortColumn = columnName;
            IsSortedAscending = true;

        }
        else //Sorting against same column but in different direction
        {
            if (IsSortedAscending)
            {
            items = items.OrderByDescending(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            }
            else
            {
                items = items.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            }

            //Toggle this boolean
            IsSortedAscending = !IsSortedAscending;
        }
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IThunderBallService ThunderBallService { get; set; }
    }
}
#pragma warning restore 1591
