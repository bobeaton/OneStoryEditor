\+DatabaseType Retellings
\ver 5.0
\+mkrset 
\lngDefault Default
\mkrRecord c

\+mkr c
\lng Default
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
\CharStyle
\-mkr

\+mkr dt
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr ln
\nam Line Number
\lng Book References
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\charset 00
\rgbColor 128,0,0
\-fnt
\mkrOverThis c
\mkrFollowingThis ret
\CharStyle
\-mkr

\+mkr ret
\nam Retelling
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\Italic
\charset 00
\rgbColor 0,128,0
\-fnt
\mkrOverThis c
\mkrFollowingThis ln
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

\-mkrset

\iInterlinCharWd 8
\+filset 

\-filset

\+jmpset 
\+jmp Verse#
\+mkrsubsetIncluded 
\mkr ln
\-mkrsubsetIncluded
\+drflst 
\+drf 
\mkr ln
\-drf
\-drflst
\-jmp
\-jmpset

\+template 
\fld \t (Title)
\fld \co Kangri
\fld \co Test 1: (name)\n
\fld \ln 01.01
\fld \ret
\fld \ln 01.02
\fld \ret
\fld \ln 01.03
\fld \ret
\fld \ln 01.04
\fld \ret
\fld \ln 01.05
\fld \ret
\fld \ln 01.06
\fld \ret
\fld \ln 01.07
\fld \ret
\fld \ln 01.08
\fld \ret
\fld \ln 01.09
\fld \ret
\fld \ln 01.10
\fld \ret
\fld \ln 01.11
\fld \ret
\fld \ln 01.12
\fld \ret
\fld \ln 01.13
\fld \ret
\fld \ln 01.14
\fld \ret
\fld \ln 01.15
\fld \ret
\fld \ln 01.16
\fld \ret
\fld \ln 01.17
\fld \ret
\fld \ln 01.18
\fld \ret
\fld \ln 01.19
\fld \ret
\fld \ln 01.20
\fld \ret
\fld \ln 01.21
\fld \ret
\fld \ln 01.22
\fld \ret
\fld \ln 01.23
\fld \ret
\fld \ln 01.24
\fld \ret
\fld \ln 01.25
\fld \ret
\fld \ln 01.26
\fld \ret
\fld \ln 01.27
\fld \ret
\fld \ln 01.28
\fld \ret
\fld \ln 01.29
\fld \ret
\fld \ln 01.30
\fld \ret
\fld \ln 01.31
\fld \ret
\fld \ln 01.32
\fld \ret
\fld \ln 01.33
\fld \ret
\fld \ln 01.34
\fld \ret
\fld \ln 01.35
\fld \ret
\fld \ln 01.36
\fld \ret
\fld \ln 01.37
\fld \ret
\fld \ln 01.38
\fld \ret
\fld \ln 01.39
\fld \ret
\fld \ln 01.40
\fld \ret
\fld \dt 12/Sep/2008
\-template
\mkrRecord c
\+PrintProperties 
\header File: &fDate: &d
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
\-expSF

\expDefault Rich Text Format
\SkipProperties
\-expset
\-DatabaseType
