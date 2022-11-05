<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HomeIndexViewModel>" %>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
    Browser Not Supported
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">

		<h1>Browser Support </h1>
        <br />
        <br />

         <table width="65%" style="HEIGHT: 100%" border="0">
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
				Your browser version is not supported at this time.  Supported browsers are listed below:
			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
                <table class="tTable">
                    <colgroup>
                        <col class="tTextLeft" />
                        <col class="tTextCenter" />
                        <col class="tTextCenter" />
                        <col class="tTextCenter" />
                    </colgroup>
                    <thead>
                        <tr>
                            <th></th>
                            <th>Windows </th>
                            <th>Mac OS </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                Internet Explorer<br />
                            </td>
                            <td>
                                6.0+
                            </td>
                            <td>
                                <span>
                                    <em>No</em>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Firefox
                            </td>
                            <td>
                                3.0+
                            </td>
                            <td>3.0+</td>
                        </tr>
                        <tr>
                            <td>
                                Google Chrome<br />
                            </td>
                            <td>
                                2.0+
                            </td>
                            <td>
                                <span>
                                    <em>No</em>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Opera
                            </td>
                            <td>
                                9.0+
                            </td>
                            <td>
                                9.0+
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Safari
                            </td>
                            <td>
                                3.0+
                            </td>
                            <td>
                                3.0+
                            </td>
                        </tr>
                    </tbody>
                </table>

                <ul>
                    <li>JavaScript must be enabled on all browsers </li>
                    <li>Caching on Internet Explorer must be activated </li>
                    <li>Browser scripting on Internet Explorer must be enabled </li>
                </ul>

			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
        </table>



</asp:Content>


