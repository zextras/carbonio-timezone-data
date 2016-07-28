## Overview

The `zm-timezones` package allows for frequent updating of timezone information
without having to build and reinstall the webapps that use this information.
The new `TzMsg*.properties` files were created by extracting the timezone
strings from the `AjxMsg*.properties` files.  The UI team should remove the
timezone strings from these files and update the following projects to load the
`TzMsg*.properties`:

- `zm-web-client`
- `zm-admin-console`

## Packaging Notes

The following artifacts from the `build` directory should be packaged so that
they are installed to one of the standard locations, such as
`/opt/zimbra/common/share/zm-timezones`.

- `bin`
- `conf`
- `WebRoot`

After installation, the post-install hook should run the `deploy-timezones`
script located in `<package-dir>/bin`.  This will copy the timezone-related
files to the proper locations and set ownership and permissions appropriately.

In addition, the post-install hooks for `zm-web-client` and `zm-admin-console`
must *also* run the `deploy-timezones` script.  If the UI team is able to
update the webapps that require timezone support to load them from paths that
are outside of the *WebRoot*, then the `deploy-timezones` hook will no longer
be required.

## Import Specification

### Inputs from Perforce

- `ZimbraServer/conf/tz`
- `ZimbraServer/conf/timezones.ics`
- `ZimbraWebClient/src/com/zimbra/kabuki/tools/tz/*`
- Timezone strings from the `AjxMsg*.properties` files located in
  `ZimbraWebClient/WebRoot/messages/AjxMsg.properties`

### Dependencies

- `zm-common`

### Artifacts

- `timezones.ics`
- `Localized timezone names`
- `AjxTimezoneData.js`
- `install-tzdata` script
