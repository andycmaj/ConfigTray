included in this release are two sample config files. they are located in the TestConfigs folder in the project folder.

config files are referenced in ConfigTray.config like so:

<file name="App"
	  path="C:\Workspace\Projects\ConfigTray\TestConfigs\app1.config"
	  watchForChanges="true">
	<settings>
		<settingName>useFiddlerProxy</settingName>
		<settingName>membershipEncryption</settingName>
	</settings>
</file>

change the path attribute to reflect the location of the project on your machine.

settingName elements correspond to setting elements in the application configuration.