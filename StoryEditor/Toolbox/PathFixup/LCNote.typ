\+DatabaseType LnC Note
\ver 5.0
\desc Language and Cultural Note
\+mkrset 
\lngDefault Default
\mkrRecord r

\+mkr b
\nam English Bible term
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\charset 00
\rgbColor 128,128,128
\-fnt
\mkrOverThis r
\-mkr

\+mkr co
\nam *
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 10
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis r
\-mkr

\+mkr dt
\nam *
\lng Date
\mkrOverThis r
\-mkr

\+mkr kt
\nam *
\lng Default
\mkrOverThis r
\-mkr

\+mkr ques
\nam question
\lng Default
\mkrOverThis r
\CharStyle
\-mkr

\+mkr r
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\charset 00
\rgbColor 0,0,0
\-fnt
\-mkr

\+mkr use
\lng Default
\mkrOverThis r
\CharStyle
\-mkr

\+mkr wrd
\nam *
\lng Vernacular
\+fnt 
\Name Arial Unicode MS
\Size 12
\charset 00
\rgbColor 0,128,128
\-fnt
\mkrOverThis r
\-mkr

\-mkrset

\iInterlinCharWd 8
\+filset 

\-filset

\+jmpset 
\+jmp KeyTerms
\+mkrsubsetIncluded 
\mkr co
\-mkrsubsetIncluded
\+drflst 
\+drf 
\File {0}\{1}\Toolbox\L&CNotes.txt
\mkr r
\-drf
\+drf 
\File {0}\{1}\Toolbox\L&CNotes.txt
\mkr wrd
\-drf
\-drflst
\-jmp
\-jmpset

\+template 
\fld \wrd
\fld \co
\fld \dt
\-template
\mkrRecord r
\mkrDateStamp dt
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
\dotFile {0}\{1}\LCNotes.dot
\exportedFile {0}\{1}\LCNotes.rtf
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
