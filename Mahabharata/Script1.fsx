//Parse name which comes before said
module Script1

#I "../packages"
#r "FsVerbalExpressions/lib/net461/FsVerbalExpressions.dll"
#r "FParsec/lib/net40-client/FParsec.dll"

let sample = """
"Sauti said, 'Having heard the diverse sacred and wonderful stories which
were composed in his Mahabharata by Krishna-Dwaipayana, and which were
recited in full by Vaisampayana at the Snake-sacrifice of the high-souled
royal sage Janamejaya and in the presence also of that chief of Princes,
the son of Parikshit, and having wandered about, visiting many sacred
waters and holy shrines, I journeyed to the country venerated by the
Dwijas (twice-born) and called Samantapanchaka where formerly was fought
the battle between the children of Kuru and Pandu, and all the chiefs of
the land ranged on either side. Thence, anxious to see you, I am come
into your presence. Ye reverend sages, all of whom are to me as Brahma;
ye greatly blessed who shine in this place of sacrifice with the
splendour of the solar fire: ye who have concluded the silent meditations
and have fed the holy fire; and yet who are sitting--without care, what,
O ye Dwijas (twice-born), shall I repeat, shall I recount the sacred
stories collected in the Puranas containing precepts of religious duty
and of worldly profit, or the acts of illustrious saints and sovereigns
of mankind?"
"""

open System.Text.RegularExpressions
open FsVerbalExpressions
open VerbalExpression
open System


                 
