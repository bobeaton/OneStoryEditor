\+DatabaseType StoryNotes
\ver 5.0
\+mkrset 
\lngDefault Default
\mkrRecord c

\+mkr apr
\nam Approval date
\lng Default
\mkrOverThis c
\-mkr

\+mkr bt
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr c
\nam Chapter
\lng Default
\-mkr

\+mkr cat
\nam Category
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 9
\Italic
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr cch
\nam Coach
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Italic
\charset 00
\rgbColor 0,0,128
\-fnt
\mkrOverThis c
\mkrFollowingThis fac
\CharStyle
\-mkr

\+mkr cn
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr co
\nam Comment
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 9
\Italic
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr con
\nam Consultant
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 10
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\mkrFollowingThis fac
\-mkr

\+mkr dt
\nam Date
\lng Date
\mkrOverThis c
\CharStyle
\-mkr

\+mkr fac
\nam Facilitator
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Italic
\charset 00
\rgbColor 128,0,128
\-fnt
\mkrOverThis con
\mkrFollowingThis fac
\-mkr

\+mkr ln
\nam Line number
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\mkrFollowingThis con
\-mkr

\+mkr p
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr rnd
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr t
\nam Title
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 10
\Bold
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr v
\nam *
\lng Default
\mkrOverThis c
\-mkr

\-mkrset

\iInterlinCharWd 8
\+filset 

\-filset

\+jmpset 
\+jmp Coaching
\+mkrsubsetIncluded 
\mkr ln
\-mkrsubsetIncluded
\+drflst 
\+drf 
\File {0}\{1}\Toolbox\ProjectConNotes.txt
\mkr ln
\-drf
\-drflst
\match_char p
\-jmp
\+jmp NetBible
\+mkrsubsetIncluded 
\mkr cch
\mkr con
\-mkrsubsetIncluded
\+drflst 
\+drf 
\File {0}\NETBIBLE.TXT
\mkr v
\-drf
\-drflst
\match_char p
\-jmp
\-jmpset

\+template 
\fld \t Title
\fld \cat Judy coaching Bob
\fld \co Kangri
\fld \cch JP1 = <date>
\fld \fac Facilitator's responses
\fld \ln 01.01
\fld \ln 01.02
\fld \ln 01.03
\fld \ln 01.04
\fld \ln 01.05
\fld \ln 01.06
\fld \ln 01.07
\fld \ln 01.08
\fld \ln 01.09
\fld \ln 01.10
\fld \ln 01.11
\fld \ln 01.12
\fld \ln 01.13
\fld \ln 01.14
\fld \ln 01.15
\fld \ln 01.16
\fld \ln 01.17
\fld \ln 01.18
\fld \ln 01.19
\fld \ln 01.20
\fld \ln 01.21
\fld \ln 01.22
\fld \ln 01.23
\fld \ln 01.24
\fld \ln 01.25
\fld \ln 01.26
\fld \ln 01.27
\fld \dt 13/Oct/2008
\-template
\mkrRecord c
\+PrintProperties 
\header File: &f, Date: &d
\footer Page &p
\topmargin 1.00 in
\leftmargin 0.25 in
\bottommargin 1.00 in
\rightmargin 0.25 in
\recordsspace 10
\-PrintProperties
\+expset 

\+expRTF Rich Text Format
\+rtfPageSetup 
\paperSize letter
\topMargin 1
\bottomMargin 1
\leftMargin 1.25
\rightMargin 1.25
\gutter 0
\headerToEdge 0.5
\footerToEdge 0.5
\columns 1
\columnSpacing 0.5
\-rtfPageSetup
\-expRTF

\+expSF Standard Format
\cctFile {0}\{1}\00-cctables\del-rtrn.cct
\exportedFile {0}\{1}\CLS Workshop Story check\Brandt fall.txt
\-expSF

\expDefault Standard Format
\CurrentRecord
\SkipProperties
\-expset
\-DatabaseType
