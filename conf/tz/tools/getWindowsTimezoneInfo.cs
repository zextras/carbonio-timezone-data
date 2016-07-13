/**
 * Based on example code from:
 * http://msdn.microsoft.com/en-us/library/system.timezoneinfo.getsystemtimezones.aspx
 */
using System;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

public class Example
{
   public static void Main()
   {
      const string OUTPUTFILENAME = @"C:\Temp\WindowsTimeZoneInfo.txt";

      CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo("en-GB"); // fix date strings as per UK culture
      DateTimeFormatInfo dateFormats = CultureInfo.CurrentCulture.DateTimeFormat;
      ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
      System.Console.WriteLine("Writing Windows timezone information to " + OUTPUTFILENAME);
      StreamWriter sw = new StreamWriter(OUTPUTFILENAME, false);

      foreach (TimeZoneInfo timeZone in timeZones)
      {
         bool hasDST = timeZone.SupportsDaylightSavingTime;
         TimeSpan offsetFromUtc = timeZone.BaseUtcOffset;
         TimeZoneInfo.AdjustmentRule[] adjustRules;
         string offsetString;

         sw.WriteLine("ID: {0}", timeZone.Id);
         sw.WriteLine("   Display Name: {0, 40}", timeZone.DisplayName);
         sw.WriteLine("   Standard Name: {0, 39}", timeZone.StandardName);
         sw.Write("   Daylight Name: {0, 39}", timeZone.DaylightName);
         sw.Write(hasDST ? "   ***Has " : "   ***Does Not Have ");
         sw.WriteLine("Daylight Saving Time***");
         offsetString = String.Format("{0} hours, {1} minutes", offsetFromUtc.Hours, offsetFromUtc.Minutes);
         sw.WriteLine("   Offset from UTC: {0, 40}", offsetString);
         adjustRules = timeZone.GetAdjustmentRules();
         sw.WriteLine("   Number of adjustment rules: {0, 26}", adjustRules.Length);
         if (adjustRules.Length > 0)
         {
            sw.WriteLine("   Adjustment Rules:");
            foreach (TimeZoneInfo.AdjustmentRule rule in adjustRules)
            {
               TimeZoneInfo.TransitionTime transTimeStart = rule.DaylightTransitionStart;
               TimeZoneInfo.TransitionTime transTimeEnd = rule.DaylightTransitionEnd;

               sw.WriteLine("      From {0} to {1}", rule.DateStart.ToString(culture.DateTimeFormat), rule.DateEnd.ToString(culture.DateTimeFormat));
               sw.WriteLine("      Delta: {0}", rule.DaylightDelta);
               string formattedBeginTime = string.Format(@"{0:HH\:mm}", transTimeStart.TimeOfDay);
               string formattedEndTime = string.Format(@"{0:HH\:mm}", transTimeEnd.TimeOfDay);
               if (! transTimeStart.IsFixedDateRule)
               {
                  sw.WriteLine("      Begins at {0} on {1} of week {2} of {3}", formattedBeginTime,
                                                                                transTimeStart.DayOfWeek,
                                                                                transTimeStart.Week,
                                                                                dateFormats.MonthNames[transTimeStart.Month - 1]);
                  sw.WriteLine("      Ends at {0} on {1} of week {2} of {3}", formattedEndTime,
                                                                                transTimeEnd.DayOfWeek,
                                                                                transTimeEnd.Week,
                                                                                dateFormats.MonthNames[transTimeEnd.Month - 1]);
               }
               else
               {
                  sw.WriteLine("      Begins at {0} on {1} {2}", formattedBeginTime,
                                                                 transTimeStart.Day,
                                                                 dateFormats.MonthNames[transTimeStart.Month - 1]);
                  sw.WriteLine("      Ends at {0} on {1} {2}", formattedEndTime,
                                                               transTimeEnd.Day,
                                                               dateFormats.MonthNames[transTimeEnd.Month - 1]);
               }
            }
         }
      }
      sw.Close();
   }
}
