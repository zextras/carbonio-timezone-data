#!/usr/bin/env python
#

from optparse import OptionParser
import sys
import codecs
import os

# parse arguments
usage = ("Usage: %prog [options]\n")
parser = OptionParser(usage)
parser.add_option("-n", "--windowsNamesFile", dest="windowsNamesFile",
        type="string", default="../windows-names",
        help="WINDOWSNAMES contains Link lines mapping Windows TZ names to Olson names(default %default)",
        metavar="WINDOWSNAMES")
parser.add_option("-e", "--extraDataFile", dest="extraDataFile",
        type="string", default="../extra-data",
        help="WTZINFO contains info about Windows Timezones (default %default)",
        metavar="WTZINFO")
parser.add_option("-i", "--windowsTimezoneInfo", dest="windowsTimezoneInfo",
        type="string", default="WindowsTimeZoneInfo.txt",
        help="WTZINFO contains info about Windows Timezones (default %default)",
        metavar="WTZINFO")

sc_name = sys.argv[0]

(options, args) = parser.parse_args()

if not os.path.exists(options.windowsNamesFile):
    print "windows-names File '{0}' does not exist".format(options.windowsNamesFile)
    sys.exit(1)
if not os.path.exists(options.extraDataFile):
    print "Extra Data File '{0}' does not exist".format(options.extraDataFile)
    sys.exit(1)
if not os.path.exists(options.windowsTimezoneInfo):
    print "Windows timezone info File '{0}' does not exist".format(options.windowsTimezoneInfo)
    sys.exit(1)

def getLinesForFile(winFile):
    inFile = codecs.open(winFile, 'r', "utf-8")
    inLines = inFile.readlines()
    return inLines

winTZInfo = getLinesForFile(options.windowsTimezoneInfo)
winDisplayNames = []
for line in winTZInfo:
    lineDetails = line.split(':')
    if lineDetails[0] == "   Display Name":
        displayName = line.strip()[13:].strip()
        dKey = '"' + displayName + '"'
        dkey2 = dKey.replace(":", ".")
        winDisplayNames.append(dkey2)

wToOlsen = {}
olsenToW = {}
error = False
windowsNamesLines = getLinesForFile(options.windowsNamesFile)
for line in windowsNamesLines:
    lineDetails = line.split('\t')
    if lineDetails[0] != "Link":
        continue
    winName = lineDetails[len(lineDetails) - 1].strip()
    if not winName in winDisplayNames:
        continue # Only want to make sure that we have entries corresponding to the main display names
    if winName.endswith('- Old"'):
        continue # Dec 2014 - '(UTC+12.00) Petropavlovsk-Kamchatsky - Old' is an example

    olsenName = lineDetails[1].strip()
    wToOlsen[winName] = olsenName
    if olsenName in olsenToW:
        prevWinName = olsenToW[olsenName]
        print "Olsen name '{0}' maps to both '{1}' and '{2}'".format(olsenName, prevWinName, winName)
        error = True
    else:
        olsenToW[olsenName] = winName

primaryZones = []
extraInfo = getLinesForFile(options.extraDataFile)
for line in extraInfo:
    lineDetails = line.split('\t')
    if (len(lineDetails) >= 2) and (lineDetails[0] == "PrimaryZone"):
        primaryZone =lineDetails[1].strip()
        if not primaryZone in primaryZones:
            primaryZones.append(primaryZone)

missingPrimaries = []
for k, v in wToOlsen.iteritems():
    if v.startswith('Etc/GMT'):
        continue
    if not v in primaryZones:
        print "Olsen '{0}' / Windows '{1}' does not have a PrimaryZone entry in '{2}'".format(v, k, options.extraDataFile)
        missingPrimaries.append(v)

if len(missingPrimaries) > 0:
    error = True
    print "Lines to add to {0}".format(options.extraDataFile)
    for zone in missingPrimaries:
        print "PrimaryZone\t{0}".format(zone)

if error:
    sys.exit(1)

sys.exit(0)
