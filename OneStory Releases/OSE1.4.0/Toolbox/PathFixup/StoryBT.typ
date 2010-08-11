\+DatabaseType StoryBT
\ver 5.0
\desc BT and Anchors. Tracks with ConNotes
\+mkrset 
\lngDefault Default
\mkrRecord c

\+mkr Ro
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr _DateStampHasFourDigitYear
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr _sh
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr anc
\nam Anchor
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\charset 00
\rgbColor 255,0,0
\-fnt
\mkrOverThis bt
\-mkr

\+mkr ans
\nam Answers
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 9
\Italic
\charset 00
\rgbColor 128,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr {0}
\nam {1}
\lng Vernacular
\mkrOverThis c
\-mkr

\+mkr bt{2}
\nam {3}
\lng NationalBT
\mkrOverThis c
\-mkr

\+mkr bt{4}
\nam {5}
\lng InternationalBT
\mkrOverThis c
\-mkr

\+mkr c
\lng Default
\-mkr

\+mkr cat
\nam Catetory
\lng Default
\mkrOverThis c
\CharStyle
\-mkr

\+mkr chk
\nam Consultant checked
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 9
\Bold
\Italic
\charset 00
\rgbColor 255,0,0
\-fnt
\mkrOverThis c
\CharStyle
\-mkr

\+mkr cn
\nam Crafting Note
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 8
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis bt
\-mkr

\+mkr co
\nam Comment
\lng InternationalBT
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
\nam *
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\Italic
\charset 00
\rgbColor 255,0,255
\-fnt
\mkrOverThis c
\-mkr

\+mkr dt
\nam Date
\lng Date
\mkrOverThis c
\-mkr

\+mkr fac
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr ft
\nam free trans.
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 10
\Italic
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr id
\nam ID of Text
\lng English
\mkrOverThis c
\-mkr

\+mkr ln
\nam Line number
\lng Book References
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\charset 00
\rgbColor 128,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr old
\nam Old version
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 8
\Italic
\charset 00
\rgbColor 0,128,0
\-fnt
\mkrOverThis c
\CharStyle
\-mkr

\+mkr p
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 10
\charset 00
\rgbColor 128,0,0
\-fnt
\mkrOverThis c
\CharStyle
\-mkr

\+mkr rec
\nam Date received
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Italic
\charset 00
\rgbColor 0,0,255
\-fnt
\mkrOverThis c
\-mkr

\+mkr s
\nam *
\lng InternationalBT
\mkrOverThis c
\-mkr

\+mkr t
\nam *
\lng InternationalBT
\+fnt 
\Name Arial Unicode MS
\Size 10
\Bold
\charset 00
\rgbColor 0,0,0
\-fnt
\mkrOverThis c
\-mkr

\+mkr thm
\nam Theme
\lng Default
\+fnt 
\Name Arial Unicode MS
\Size 8
\Bold
\charset 00
\rgbColor 0,0,255
\-fnt
\mkrOverThis c
\-mkr

\+mkr to-do
\nam *
\lng Default
\mkrOverThis c
\-mkr

\+mkr tst{0}
\nam testing qs in vernacular
\lng Vernacular
\+fnt 
\Name {6}
\Size 9
\Bold
\charset 00
\rgbColor 0,0,255
\-fnt
\mkrOverThis c
\CharStyle
\-mkr

\+mkr tst{4}
\nam testing qs in English
\lng InternationalBT
\+fnt 
\Name {7}
\Size 9
\Bold
\charset 00
\rgbColor 0,64,0
\-fnt
\mkrOverThis c
\CharStyle
\-mkr

\-mkrset

\iInterlinCharWd 8
\+filset 

\-filset

\+jmpset 
\+jmp Cultural Notes
\+mkrsubsetIncluded 
\mkr cn
\-mkrsubsetIncluded
\+drflst 
\-drflst
\match_char p
\-jmp
\+jmp Keyterms
\+mkrsubsetIncluded 
\mkr bt
\-mkrsubsetIncluded
\+drflst 
\-drflst
\match_char p
\-jmp
\+jmp LineNumber
\+mkrsubsetIncluded 
\mkr ln
\-mkrsubsetIncluded
\+drflst 
\+drf 
\File {8}\{9}\Toolbox\ProjectConNotes.txt
\mkr ln
\-drf
\-drflst
\-jmp
\+jmp Net Bible
\+mkrsubsetIncluded 
\mkr anc
\-mkrsubsetIncluded
\+drflst 
\+drf 
\File {8}\NETBIBLE.TXT
\mkr v
\-drf
\-drflst
\match_char p
\-jmp
\-jmpset

\mkrRecord c
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
\exportedFile {8}\{9}\{9}.rtf
\MarkerFont
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
\exportedFile {8}\{9}\{9}.sfm
\-expSF

\expDefault Standard Format
\CurrentRecord
\AutoOpen
\SkipProperties
\-expset
\HideFields
\-DatabaseType
