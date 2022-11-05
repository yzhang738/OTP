<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsManageViewModel>" %>

    <div>

        <div style="float:left;width:39%;text-align:right;margin:0px 0px 5px 0px;">
            <%: Shared.ResultsStrings.ResultsStatusSelector_Status %>
        </div>
        <div style="float:right;width:60%;text-align:left;margin:0px 0px 5px 0px;">

            <% using (Html.BeginForm("ManageResults", "Results",FormMethod.Get, new { name = "frmResultManageStatus" }))
               { %>
                <%= Html.Telerik().DropDownList()
                    .Name("resultsFilter")
                    .HtmlAttributes(new { style = "width:250px;font-size:12px" })
                    .BindTo(new SelectList(Model.SelectorItems, "Id", "Description", Model.SelectorValue))
                    .ClientEvents(e => e.OnChange("changeResultManageStatus"))
                      
                %>
            <% } %>

        </div> 
        
        <div>
            <script type="text/javascript">

                function changeResultManageStatus()
                {
                    document.frmResultManageStatus.submit();
                    ShowPleaseWait();
                };

            </script>
        </div>

    </div>