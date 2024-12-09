# mathsemantics-semantic package
# Matthew Bertucci 2022/05/08 for v1.0.0

#include:mathsemantics-commons
#include:mathsemantics-syntax

\abs{arg}#m
\abs[scale%keyvals]{arg}#m
\ceil{arg}#m
\ceil[scale%keyvals]{arg}#m
\dual{arg1}{arg2}#m
\dual[scale%keyvals]{arg1}{arg2}#m
\floor{arg}#m
\floor[scale%keyvals]{arg}#m
\avg{arg}#m
\avg[scale%keyvals]{arg}#m
\inner{arg1}{arg2}#m
\inner[scale%keyvals]{arg1}{arg2}#m
\jump{arg}#m
\jump[scale%keyvals]{arg}#m
\norm{arg}#m
\norm[scale%keyvals]{arg}#m
\restr{arg}{sub}#m
\restr[scale%keyvals]{arg}{sub}#m
\setMid#*
\setDef{arg1}{arg2}#m
\setDef[scale%keyvals]{arg1}{arg2}#m

\distOp#*m
\dist#m
\dist{sub}#*m
\dist{sub}{arg}#m
\dist[scale%keyvals]#m
\dist[scale%keyvals]{sub}#*m
\dist[scale%keyvals]{sub}{arg}#m
\projOp#*m
\proj#m
\proj{sub}#*m
\proj{sub}(arg)#m
\proj[scale%keyvals]#m
\proj[scale%keyvals]{sub}#*m
\proj[scale%keyvals]{sub}(arg)#m
\proxOp#*m
\prox#m
\prox{sub}#*m
\prox{sub}(arg)#m
\prox[scale%keyvals]#m
\prox[scale%keyvals]{sub}#*m
\prox[scale%keyvals]{sub}(arg)#m

\aff#m
\arcosh#m
\arcoth#m
\arsinh#m
\artanh#m
\argmax#m
\Argmax#m
\argmin#m
\Argmin#m
\bdiv#m
\card#m
\clconv#m
\closure#m
\cofac#m
\compactly#m
\cone#m
\conv#m
\corresponds#m
\cov#m
\curl#m
\dev#m
\div#m
\Div#m
\dInt#*m
\d
\diag#m
\diam#m
\dom#m
\dotcup#m
\dprod#m
\e
\embed#m
\embeds#*m
\epi#m
\eR#m
\esssup#m
\essinf#m
\grad#m
\Graph#m
\id
\image#m
\interior#m
\inj#m
\laplace#m
\limessinf#m
\limesssup#m
\lin#m
\rank#m
\range#m
\ri#m
\sgn#m
\Sgn#m
\Span#m
\supp#m
\sym#m
\trace#m
\transposeSymbol#*m
\transp#m
\var#m
\weakly#m
\weaklystar#m
\orcid{ORCID}

#keyvals:\abs#c,\ceil#c,\dual#c,\floor#c,\avg#c,\inner#c,\jump#c,\norm#c,\restr#c,\setDef#c,\dist#c,\proj#c,\prox#c
big
Big
bigg
Bigg
auto
none
#endkeyvals