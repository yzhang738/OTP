<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsSummaryViewModel>" %>


    <div>

        <div style="float:left;width:49%;text-align:right;margin:0px 0px 5px 0px;">
            <%: Shared.ResultsStrings.SummaryStatusSelector_Status %>
        </div>
        <div style="float:right;width:50%;text-align:left;margin:0px 0px 5px 0px;">

            <% using (Html.BeginForm("Summary", "Results",FormMethod.Get, new { name = "frmResultSummaryStatus" }))
               { %>
                <%= Html.Telerik().DropDownList()
                    .Name("summaryFilter")
                    .HtmlAttributes(new { style = "width:175px;font-size:12px" })
                    .BindTo(new SelectList(Model.SelectorItems, "Id", "Description", Model.SelectorValue))
                    .ClientEvents(e => e.OnChange("changeResultSummaryStatus"))
                %>
            <% } %>

        </div> 
        
        <div>
            <script type="text/javascript">

                function changeResultSummaryStatus() {
                    document.frmResultSummaryStatus.submit();
                };

            </script>
        </div>

    </div>

