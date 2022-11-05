﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesContactInfoViewModel>" %>
        
    <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />
    <% using (Html.BeginForm("AthletesContactInfoSave", "Maintenance", new { id = ViewData["resourceId"] }, FormMethod.Post))
       {
           if (Model != null)
           { %>
        <%: Html.ValidationSummary(true)%>

        <div>
            <div class="groupbox_1000" style="margin:0px 0px 0px 0px">
                <div class="groupbox_title_background gradientHToWhite">
                    <div class="groupbox_title" >
                       <%= ViewData["resourceName"] %>
                    </div>
                </div>
                <div class="groupbox_content">

                    <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                        <div style="float:left; width:64%;margin:0px 20px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.EMailAddress)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.EMailAddress, new { style = "width:99%"})%>
                                <%: Html.ValidationMessageFor(model => model.EMailAddress)%>
                            </div>
                        </div> 

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.SkypeName)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.SkypeName, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.SkypeName)%>
                            </div>
                        </div> 
                    </div>

                    <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.HometownCity)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.HometownCity, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.HometownCity)%>
                            </div>
                        </div>

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.HometownProvinceId)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.DropDownList("HometownProvinceId", new SelectList(Model.Provinces, "Id", "Description", Model.HometownProvinceId), new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.HometownProvinceId)%>
                            </div>
                        </div>

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.HometownCountryId)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.DropDownList("HometownCountryId", new SelectList(Model.Countries, "Id", "Description", Model.HometownCountryId), new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.HometownCountryId)%>
                            </div>
                        </div>
                    </div>

                    <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                        <div style="float:left; width:64%;margin:0px 20px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.MailingAddress)%>
                            </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.MailingAddress, new { style = "width:99%"})%>
                                    <%: Html.ValidationMessageFor(model => model.MailingAddress)%>
                            </div>
                        </div> 

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.MailingCity)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.MailingCity, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.MailingCity)%>
                            </div>
                        </div> 
                    </div>

                    <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.MailingPostalCode)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.MailingPostalCode, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.MailingPostalCode)%>
                            </div>
                        </div>

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.MailingProvinceId)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.DropDownList("MailingProvinceId", new SelectList(Model.Provinces, "Id", "Description", Model.MailingProvinceId), new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.MailingProvinceId)%>
                            </div>
                        </div>

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.MailingCountryId)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.DropDownList("MailingCountryId", new SelectList(Model.Countries, "Id", "Description", Model.MailingCountryId), new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.MailingCountryId)%>
                            </div>
                        </div>
                    </div>

                    <div style="float:left; width:98%;margin:0px 0px 15px 0px;">
                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.HomePhone)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.HomePhone, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.HomePhone)%>
                            </div>
                        </div>

                        <div style="float:left; width:32%;margin:0px 10px 0px 0px;">
                            <div class="editor-label">
                                <%: Html.LabelFor(model => model.CellPhone)%>
                            </div>
                            <div class="editor-field">
                                <%: Html.TextBoxFor(model => model.CellPhone, new { style = "width:99%" })%>
                                <%: Html.ValidationMessageFor(model => model.CellPhone)%>
                            </div>
                        </div>
                    </div>

                </div>
                <img src="../../Content/Images/sidenav_btm.gif" width="1000" height="9" /> 
        
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
                       <%= ViewData["resourceName"] %>
                    </div>
                </div>
           <% }
       } %>


