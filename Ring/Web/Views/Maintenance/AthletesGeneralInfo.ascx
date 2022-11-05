﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesGeneralInfoViewModel>" %>
        <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

    <% using (Html.BeginForm("AthletesGeneralInfoSave", "Maintenance", new { id = ViewData["resourceId"] }, FormMethod.Post))
       {
           if (Model != null)
           {%>
        <%: Html.ValidationSummary(true)%>


        <div>
            <div class="groupbox_1000" style="margin:0px 0px 0px 0px">
                <div class="groupbox_title_background gradientHToWhite">
                    <div class="groupbox_title" >
                       <%= ViewData["resourceName"]%>
                    </div>
                </div>
                <div class="groupbox_content">
                    
                    <div style="float:left; width:49%;">
                        <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                            <div style="float:left; width:49%;">
                                <div class="editor-label">
                                     <%: Html.LabelFor(model => model.FirstName)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.FirstName, new { style = "width:99%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.FirstName)%>
                                </div>
                            </div>

                            <div style="float:right; width:49%;">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.LastName)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.LastName, new { style = "width:99%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.LastName)%>
                                </div>
                            </div>

                        </div>

                        <div style="float:left; width:98%; width:98%;margin:0px 0px 15px 0px;">
                            <div style="float:left; width:49%;">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.CountryId)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.DropDownList("CountryId", new SelectList(Model.Countries, "Id", "Description", Model.CountryId), new { style = "width:99%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.CountryId)%>
                                </div>
                            </div>

                            <div style="float:right; width:24%;">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.DateOfBirth)%>
                                </div>
                                <div class="editor-field">
                                    <%= Html.Telerik().DatePicker()
                                            .Name("DateOfBirth")
                                            .Min(DateTime.Now.AddYears(-120))
                                            .Max(DateTime.Now.AddSeconds(1))
                                            .Value(Model.DateOfBirth)
                                            .ShowButton(true)
                                            .HtmlAttributes(new { style = "width:99%;" })
                                    %>
                            
                                    <%: Html.ValidationMessageFor(model => model.DateOfBirth)%>
                                </div>
                            </div>

                            <div style="float:right; width:23%;margin: 0px 10px 0px 0px">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.Gender)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.DropDownList("Gender", new SelectList(Model.Genders, "Id", "Description", Model.Gender), new { style = "width:99%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.Gender)%>
                                </div>
                            </div>
                        </div>

                        <div style="float:left; width:98%; width:98%;margin:0px 0px 15px 0px;">
                            <fieldset>
                                <legend><%: Shared.MaintenanceStrings.AthletesGeneralInfo_CSC%></legend>
                            
                                <div style="float:left; width:47%;margin: 10px 0px 10px 10px;">
                                    <div class="editor-label">
                                        <%: Html.LabelFor(model => model.PrimaryCSCId)%>
                                    </div>
                                    <div class="editor-field">
                                        <%: Html.DropDownList("PrimaryCSCId", new SelectList(Model.CSCs, "Id", "Description", Model.PrimaryCSCId), new { style = "width:99%;" })%>
                                        <%: Html.ValidationMessageFor(model => model.PrimaryCSCId)%>
                                    </div>
                                </div>

                                <div style="float:right; width:47%;margin:10px 10px 10px 0px">
                                    <div class="editor-label">
                                        <%: Html.LabelFor(model => model.SecondaryCSCId)%>
                                    </div>
                                    <div class="editor-field">
                                        <%: Html.DropDownList("SecondaryCSCId", new SelectList(Model.CSCs, "Id", "Description", Model.SecondaryCSCId), new { style = "width:99%;" })%>
                                        <%: Html.ValidationMessageFor(model => model.SecondaryCSCId)%>
                                    </div>
                                </div>
                            </fieldset>
                        </div>

                        <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                            <div style="float:left; width:49%;">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.PersonalCoachId)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.DropDownList("PersonalCoachId", new SelectList(Model.CoachResources, "Id", "Description", Model.PersonalCoachId), new { style = "width:99%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.PersonalCoachId)%>
                                </div>
                            </div>

                            <div style="float:right; width:40%;margin:13px 0px 0px 10px">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.IsNew)%>
                                    <%: Html.CheckBox("IsNew", Model.IsNew, new { style = "width:10%;" })%>
                                    <%: Html.ValidationMessageFor(model => model.IsNew)%>
                                </div>
                                <div class="editor-field">

                                </div>
                            </div>

                        </div>

                    </div>

                    <div style="float:right; width:49%">
                        <div class="editor-label">
                            <%: Html.LabelFor(model => model.Comments)%>
                        </div>
                        <div class="editor-field">
                            <%=
                                Html.Telerik().Editor()
                                    .Name("comments")
                                    .Tools(t => t.Clear())
                                    .Value(Model.Comments)
                                    .HtmlAttributes(new { style = "height:175px" })
                            %>

                            <%: Html.ValidationMessageFor(model => model.Comments)%>
                        </div>
                    </div>

                </div>
                <img src="../../Content/Images/sidenav_btm.gif" width="1100" height="9" /> 

            <% if (Model.Editable)
               { %>
                <div style="margin: 5px 20px 20px 5px; float:right;" >
                    <input type="submit" value="<%: Shared.SharedStrings.Button_Save %>" />
                </div>
            <% } %>
            </div> 

            <div class="clear"></div>

        </div>
        
        <% }
           else
           { %>
                <div class="groupbox_title_background gradientHToWhite">
                    <div class="groupbox_title" >
                       <%= ViewData["resourceName"]%>
                    </div>
                </div>
           <% }
       } %>


