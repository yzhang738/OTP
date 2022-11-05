<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="OTPAzure" generation="1" functional="0" release="0" Id="02150c2d-b634-404c-a3f4-1b9a4619921f" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="OTPAzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="OTP.Ring.Web:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/OTPAzure/OTPAzureGroup/LB:OTP.Ring.Web:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="OTP.Ring.Web:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:debugLiveId" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:debugLiveId" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultCompetitionID" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultCompetitionID" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultCompetitionTypeID" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultCompetitionTypeID" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultCountry" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultCountry" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultCountryID" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultCountryID" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultHostCity" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultHostCity" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultSportID" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultSportID" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:defaultYear" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:defaultYear" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:loggingLevel" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:loggingLevel" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:ReportConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:ReportConnectionString" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:RingEntities" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:RingEntities" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:siteTheme" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:siteTheme" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:useDebugLiveId" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:useDebugLiveId" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:wll_appid" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:wll_appid" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:wll_secret" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:wll_secret" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.Web:wll_securityalgorithm" defaultValue="">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.Web:wll_securityalgorithm" />
          </maps>
        </aCS>
        <aCS name="OTP.Ring.WebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/OTPAzure/OTPAzureGroup/MapOTP.Ring.WebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:OTP.Ring.Web:Endpoint1">
          <toPorts>
            <inPortMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapOTP.Ring.Web:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/DataConnectionString" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:debugLiveId" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/debugLiveId" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultCompetitionID" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultCompetitionID" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultCompetitionTypeID" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultCompetitionTypeID" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultCountry" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultCountry" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultCountryID" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultCountryID" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultHostCity" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultHostCity" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultSportID" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultSportID" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:defaultYear" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/defaultYear" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:loggingLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/loggingLevel" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:ReportConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/ReportConnectionString" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:RingEntities" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/RingEntities" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:siteTheme" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/siteTheme" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:useDebugLiveId" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/useDebugLiveId" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:wll_appid" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/wll_appid" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:wll_secret" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/wll_secret" />
          </setting>
        </map>
        <map name="MapOTP.Ring.Web:wll_securityalgorithm" kind="Identity">
          <setting>
            <aCSMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web/wll_securityalgorithm" />
          </setting>
        </map>
        <map name="MapOTP.Ring.WebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.WebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="OTP.Ring.Web" generation="1" functional="0" release="0" software="C:\Users\zhang\Documents\Visual Studio 2010\Projects\OTP\Ring\Application\trunk\OTP\Ring\OTPAzure\csx\Debug\roles\OTP.Ring.Web" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="debugLiveId" defaultValue="" />
              <aCS name="defaultCompetitionID" defaultValue="" />
              <aCS name="defaultCompetitionTypeID" defaultValue="" />
              <aCS name="defaultCountry" defaultValue="" />
              <aCS name="defaultCountryID" defaultValue="" />
              <aCS name="defaultHostCity" defaultValue="" />
              <aCS name="defaultSportID" defaultValue="" />
              <aCS name="defaultYear" defaultValue="" />
              <aCS name="loggingLevel" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="ReportConnectionString" defaultValue="" />
              <aCS name="RingEntities" defaultValue="" />
              <aCS name="siteTheme" defaultValue="" />
              <aCS name="useDebugLiveId" defaultValue="" />
              <aCS name="wll_appid" defaultValue="" />
              <aCS name="wll_secret" defaultValue="" />
              <aCS name="wll_securityalgorithm" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;OTP.Ring.Web&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;OTP.Ring.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.WebInstances" />
            <sCSPolicyUpdateDomainMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.WebUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.WebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="OTP.Ring.WebUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="OTP.Ring.WebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="OTP.Ring.WebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="8683bc77-0cbc-4517-8713-1bed58b90b8a" ref="Microsoft.RedDog.Contract\ServiceContract\OTPAzureContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="4a0d00a1-515b-41da-a158-41542ae1b5f8" ref="Microsoft.RedDog.Contract\Interface\OTP.Ring.Web:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/OTPAzure/OTPAzureGroup/OTP.Ring.Web:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>