# Provider App

The Provider App is located in the AABC.Web repository.  There are two solutions in that repo: one for the Manage code, and one for the Provider App.  Some projects are shared between the two repositories.

* AABC.Web.sln (Manage related code)
* AABC.Mobile.sln (Provider App and related)

The following projects are shared between both solutiuons:

* AABC.Domain
* AABC.Data
* AABC.Domain2
* AABC.DomainServices
* AABC.Shared.Web

The AABC.Mobile solution contains the Provider App API and the Xamarin Android/iOS projects.

## Versioning

The client app includes a version update check.  Therefore, when releasing either the server or client components, it's important to make sure the versions are incremented accordingly prior to release.

### Serverside Versions
Locate `AABC.Mobile.Api.Web.config` file of the project and set the `appSetting` key `Version.Server` as required.

Additionally, set the minimum supported client version here via the `Version.Client.Minimum` key.

### Clientside Versions

The Android and iOS version lives in different places:

* Android: `AABC.Mobile.Android` project, expand Properties and find `Android.Manifest.xml`.  Update the attribute `android:versionName` as required with a **4-part version** (the versionCode should be incremented for each build).  This can also be set via the Properties UI in Visual Studio.  
* iOS: `AABC.Mobile.iOS` project, open the `Info.plist` file.  Locate the `CFBundleVersion` key and set the string value to a **3-part version** identifier. 

## Deployment

(see also: https://docs.microsoft.com/en-us/xamarin/android/deploy-test/release-prep/?tabs=vswin)

### Android

1. Ensure we're in Release mode.
2. Update AABC.Mobile.Android properties and increment the Version Code and Version Number
3. Archive the application
4. Sign using Dymeng.AppliedAbc.Android key (see Keeper for key password)
5. Copy file to applied's server, place in `\Files\MobileAppArchive\com.appliedabc.mobile.provider.app`
6. Optionally place a copy in `\Sites\com.appliedabc.downloads`
