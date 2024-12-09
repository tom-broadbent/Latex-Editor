# jmlr class
# Matthew Bertucci 3/25/2022 for v1.30

#include:xkeyval
#include:calc
#include:etoolbox
#include:placeins
#include:amsmath
#include:amssymb
#include:natbib
#include:graphicx
#include:url
#include:xcolor
# loads x11names option of xcolor
#include:algorithm2e
# loads algo2e option of algorithm2e
#include:jmlrutils
#include:hyperref
#include:nameref

#keyvals:\documentclass/jmlr#c
color
gray
draft
final
tablecaptiontop
tablecaptionbottom
10pt
11pt
12pt
nowcp
wcp
pmlr
cleveref
oneside
twoside
7x10
letterpaper
#endkeyvals

#ifOption:cleveref
#include:aliascnt
#include:cleveref
#endif

\abovestrut{height}
\acks{acknowledgements%text}
\addr
\aftermaketitskip#*
\aftertitskip#*
\artappendix#*
\artchapter#S
\artpart*{title}#*L0
\artpart[short title]{title}#*L0
\artpart{title}#*L0
\arttableofcontents#*
\author
\backmatter#*
\beforetitskip#*
\begin{keywords}
\belowstrut{height}
\bookappendix#*
\bookchapter*{title}#*L1
\bookchapter[short title]{title}#*L1
\bookchapter{title}#*L1
\booklinebreak{number}#*
\bookpart*{title}#*L0
\bookpart[short title]{title}#*L0
\bookpart{title}#*L0
\booktableofcontents#*
\booktocpostamble#*
\booktocpreamble#*
\chapter*{title}#*L1
\chapter[short title]{title}#*L1
\chapter{title}#*L1
\chapterformat#*
\chaptermark{code}#*
\chaptername#*
\chapternumberformat{text}#*
\chaptertitleformat{text}#*
\editor{name}
\editorname#*
\editors{names}
\editorsname#*
\Email{email%URL}#U
\end{keywords}
\figurecaption{text1%text}{text2%text}#*
\figurecenter{text1%text}{text2%text}#*
\firstpageno{number}#*
\footnoteseptext#*
\frontmatter#*
\grayscalefalse#*
\grayscaletrue#*
\ifgrayscale#*
\ifprint{true}{false}
\ifviiXx#*
\interauthorskip#*
\jmlrabbrnamelist{names}#*
\jmlrarticlecommands#*
\jmlrauthorhook#*
\jmlrbookcommands#*
\jmlrbox#*
\jmlrcheckforpseudocode#*
\jmlrhtmlmaketitle#*
\jmlrissue{number}#*
\jmlrlength#*
\jmlrmaketitle#*
\jmlrmaketitlehook#*
\jmlrnowcp#*
\jmlrpages{pages}#*
\jmlrpmlr#*
\jmlrpostauthor#*
\jmlrposttitle#*
\jmlrpreauthor#*
\jmlrpremaketitlehook#*
\jmlrpretitle#*
\jmlrproceedings{short title%text}{long title%text}#*
\jmlrpublished{date}
\jmlrsubmitted{date}
\jmlrSuppressPackageChecks#*
\jmlrtitlehook#*
\jmlrvolume{number}
\jmlrwcp#*
\jmlrworkshop{workshop title%text}
\jmlryear{year}
\kernelmachines#*
\mainmatter#*
\morefrontmatter#*
\moremainmatter#*
\Name{name}
\nametag{text}
\obsoletefontcs{csname}#*
\partformat#*
\partnumberformat{text}#*
\parttitleformat{text}#*
\postchapterskip#*
\postparthook#*
\prechapterskip#*
\preparthook#*
\presectionnum#*
\reprint{arg}#*
\researchnote{text}
\thechapter#*
\titlebreak
\titletag{text}
\viiXxfalse#*
\viiXxtrue#*

# only available if pseudocode loaded
\ENDFOR#S
\pseudoAND#S
\pseudoCOMMENT#S
\pseudoELSE#S
\pseudoENDFOR#S
\pseudoFALSE#S
\pseudoFOR#S
\pseudoFORALL#S
\pseudoIF#S
\pseudoNOT#S
\pseudoOR#S
\pseudoREPEAT#S
\pseudoRETURN#S
\pseudoTO#S
\pseudoTRUE#S
\pseudoUNTIL#S
\pseudoWHILE#S

# from x11names option of xcolor
AntiqueWhite1#B
AntiqueWhite2#B
AntiqueWhite3#B
AntiqueWhite4#B
Aquamarine1#B
Aquamarine2#B
Aquamarine3#B
Aquamarine4#B
Azure1#B
Azure2#B
Azure3#B
Azure4#B
Bisque1#B
Bisque2#B
Bisque3#B
Bisque4#B
Blue1#B
Blue2#B
Blue3#B
Blue4#B
Brown1#B
Brown2#B
Brown3#B
Brown4#B
Burlywood1#B
Burlywood2#B
Burlywood3#B
Burlywood4#B
CadetBlue1#B
CadetBlue2#B
CadetBlue3#B
CadetBlue4#B
Chartreuse1#B
Chartreuse2#B
Chartreuse3#B
Chartreuse4#B
Chocolate1#B
Chocolate2#B
Chocolate3#B
Chocolate4#B
Coral1#B
Coral2#B
Coral3#B
Coral4#B
Cornsilk1#B
Cornsilk2#B
Cornsilk3#B
Cornsilk4#B
Cyan1#B
Cyan2#B
Cyan3#B
Cyan4#B
DarkGoldenrod1#B
DarkGoldenrod2#B
DarkGoldenrod3#B
DarkGoldenrod4#B
DarkOliveGreen1#B
DarkOliveGreen2#B
DarkOliveGreen3#B
DarkOliveGreen4#B
DarkOrange1#B
DarkOrange2#B
DarkOrange3#B
DarkOrange4#B
DarkOrchid1#B
DarkOrchid2#B
DarkOrchid3#B
DarkOrchid4#B
DarkSeaGreen1#B
DarkSeaGreen2#B
DarkSeaGreen3#B
DarkSeaGreen4#B
DarkSlateGray1#B
DarkSlateGray2#B
DarkSlateGray3#B
DarkSlateGray4#B
DeepPink1#B
DeepPink2#B
DeepPink3#B
DeepPink4#B
DeepSkyBlue1#B
DeepSkyBlue2#B
DeepSkyBlue3#B
DeepSkyBlue4#B
DodgerBlue1#B
DodgerBlue2#B
DodgerBlue3#B
DodgerBlue4#B
Firebrick1#B
Firebrick2#B
Firebrick3#B
Firebrick4#B
Gold1#B
Gold2#B
Gold3#B
Gold4#B
Goldenrod1#B
Goldenrod2#B
Goldenrod3#B
Goldenrod4#B
Green1#B
Green2#B
Green3#B
Green4#B
Honeydew1#B
Honeydew2#B
Honeydew3#B
Honeydew4#B
HotPink1#B
HotPink2#B
HotPink3#B
HotPink4#B
IndianRed1#B
IndianRed2#B
IndianRed3#B
IndianRed4#B
Ivory1#B
Ivory2#B
Ivory3#B
Ivory4#B
Khaki1#B
Khaki2#B
Khaki3#B
Khaki4#B
LavenderBlush1#B
LavenderBlush2#B
LavenderBlush3#B
LavenderBlush4#B
LemonChiffon1#B
LemonChiffon2#B
LemonChiffon3#B
LemonChiffon4#B
LightBlue1#B
LightBlue2#B
LightBlue3#B
LightBlue4#B
LightCyan1#B
LightCyan2#B
LightCyan3#B
LightCyan4#B
LightGoldenrod1#B
LightGoldenrod2#B
LightGoldenrod3#B
LightGoldenrod4#B
LightPink1#B
LightPink2#B
LightPink3#B
LightPink4#B
LightSalmon1#B
LightSalmon2#B
LightSalmon3#B
LightSalmon4#B
LightSkyBlue1#B
LightSkyBlue2#B
LightSkyBlue3#B
LightSkyBlue4#B
LightSteelBlue1#B
LightSteelBlue2#B
LightSteelBlue3#B
LightSteelBlue4#B
LightYellow1#B
LightYellow2#B
LightYellow3#B
LightYellow4#B
Magenta1#B
Magenta2#B
Magenta3#B
Magenta4#B
Maroon1#B
Maroon2#B
Maroon3#B
Maroon4#B
MediumOrchid1#B
MediumOrchid2#B
MediumOrchid3#B
MediumOrchid4#B
MediumPurple1#B
MediumPurple2#B
MediumPurple3#B
MediumPurple4#B
MistyRose1#B
MistyRose2#B
MistyRose3#B
MistyRose4#B
NavajoWhite1#B
NavajoWhite2#B
NavajoWhite3#B
NavajoWhite4#B
OliveDrab1#B
OliveDrab2#B
OliveDrab3#B
OliveDrab4#B
Orange1#B
Orange2#B
Orange3#B
Orange4#B
OrangeRed1#B
OrangeRed2#B
OrangeRed3#B
OrangeRed4#B
Orchid1#B
Orchid2#B
Orchid3#B
Orchid4#B
PaleGreen1#B
PaleGreen2#B
PaleGreen3#B
PaleGreen4#B
PaleTurquoise1#B
PaleTurquoise2#B
PaleTurquoise3#B
PaleTurquoise4#B
PaleVioletRed1#B
PaleVioletRed2#B
PaleVioletRed3#B
PaleVioletRed4#B
PeachPuff1#B
PeachPuff2#B
PeachPuff3#B
PeachPuff4#B
Pink1#B
Pink2#B
Pink3#B
Pink4#B
Plum1#B
Plum2#B
Plum3#B
Plum4#B
Purple1#B
Purple2#B
Purple3#B
Purple4#B
Red1#B
Red2#B
Red3#B
Red4#B
RosyBrown1#B
RosyBrown2#B
RosyBrown3#B
RosyBrown4#B
RoyalBlue1#B
RoyalBlue2#B
RoyalBlue3#B
RoyalBlue4#B
Salmon1#B
Salmon2#B
Salmon3#B
Salmon4#B
SeaGreen1#B
SeaGreen2#B
SeaGreen3#B
SeaGreen4#B
Seashell1#B
Seashell2#B
Seashell3#B
Seashell4#B
Sienna1#B
Sienna2#B
Sienna3#B
Sienna4#B
SkyBlue1#B
SkyBlue2#B
SkyBlue3#B
SkyBlue4#B
SlateBlue1#B
SlateBlue2#B
SlateBlue3#B
SlateBlue4#B
SlateGray1#B
SlateGray2#B
SlateGray3#B
SlateGray4#B
Snow1#B
Snow2#B
Snow3#B
Snow4#B
SpringGreen1#B
SpringGreen2#B
SpringGreen3#B
SpringGreen4#B
SteelBlue1#B
SteelBlue2#B
SteelBlue3#B
SteelBlue4#B
Tan1#B
Tan2#B
Tan3#B
Tan4#B
Thistle1#B
Thistle2#B
Thistle3#B
Thistle4#B
Tomato1#B
Tomato2#B
Tomato3#B
Tomato4#B
Turquoise1#B
Turquoise2#B
Turquoise3#B
Turquoise4#B
VioletRed1#B
VioletRed2#B
VioletRed3#B
VioletRed4#B
Wheat1#B
Wheat2#B
Wheat3#B
Wheat4#B
Yellow1#B
Yellow2#B
Yellow3#B
Yellow4#B
Gray0#B
Green0#B
Grey0#B
Maroon0#B
Purple0#B

# from algo2e option of algorithm2e
\begin{algorithm2e}
\begin{algorithm2e}[placement]
\end{algorithm2e}
\begin{algorithm2e*}#*
\begin{algorithm2e*}[placement]#*
\end{algorithm2e*}#*
\listofalgorithmes

# deprecated
\ifjmlrhtml#S
\jmlrhtmltrue#S
\jmlrhtmlfalse#S