#!/usr/bin/perl -w

use Getopt::Std;
use Env;
use File::Basename;
use File::stat;
use File::Copy;

$sc_name              = basename("$0");
$usage                = "usage: $sc_name -t timezones.ics [-p TzMsg.properties]\n";

getopts('t:p:') or die "$usage";

local $ajxprops = "../../../messages/TzMsg.properties";

die "$usage" if (!$opt_t);
$tzics = "$opt_t";
if ($opt_p) {
    $ajxprops = $opt_p;
};

open(TZF, "$tzics") || die "No $tzics file - exit";
open(PROPS, "$ajxprops") || die "No $ajxprops file - exit";

%tzoffsetinfo = ();

$inComponent = "n";
$tzname = "UNSET";

while (<TZF>)
{
    /^TZID:/ && do {
        $line = $_;
        chomp $line;
        @bits = split(":", $line);
        $tzname = $bits[1];
        next;
    };

    /^TZOFFSETTO:/ && do {
        $line = $_;
        chomp $line;
        next if ($inComponent eq "n");
        @bits = split(":", $line);
        $tzoffsetto = $bits[1];
        $tzoffsetinfo { "$tzname" } = "$tzoffsetto";
        next;
    };

    /^BEGIN:STANDARD/ && do {
        $inComponent = "y";
        next;
    };

    /^END:STANDARD/ && do {
        $inComponent = "n";
        next;
    };

}
close(TZF);

%propinfo = ();

while (<PROPS>)
{
    /GMT/ && do {
        $line = $_;
        chomp $line;
        ($key, $tzname) = split(' = ', $line);
        $propinfo { "$key" } = "$tzname";
        next;
    };
}
close(PROPS);

$problems = "n";
foreach $my_prop (sort keys(%propinfo)) {
    next if ($my_prop eq "UTC");
    if (exists $tzoffsetinfo{"$my_prop"}) {
        $icstzoffsetto =$tzoffsetinfo{"$my_prop"};
        $desc = $propinfo {"$my_prop"};
        $proptzoffset = $desc;
        # ? for non-greedy matching
        $proptzoffset =~ s/.*?GMT *//;
        $proptzoffset =~ s/://g;
        $proptzoffset =~ s/ .*$//g;
        $proptzoffset =~ s/\)//g;
        if ($proptzoffset ne $icstzoffsetto) {
            if ($problems eq "n") {
                print "TIMEZONES in ".$ajxprops." with incorrect GMT offsets:\n";
            };
            $problems = "y";
            print "proptzoffset=".$proptzoffset." icstzoffsetto=".$icstzoffsetto."\n";
            print "KEY='".$my_prop."' timezones.ics offset=".$icstzoffsetto." PropertyValue='".$desc."'\n";
        }
    }
}
if ($problems eq "n") {
    print "No problems found with $ajxprops\n";
} else {
    exit 1;
}
exit 0;

__END__
