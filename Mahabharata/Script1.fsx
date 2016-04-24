//Parse name which comes before said
// open VerbalExpression
//can't make it work with string if I am passing "a said b" then it should return (a,b)
//fall back to string split. 
//TODO use regex or FParsec for better performace
module Script1

#I "../packages"
#r "FsVerbalExpressions/lib/net461/FsVerbalExpressions.dll"
#r "FParsec/lib/portable-net45+netcore45+wpa81+wp8/FParsecCS.dll"
#r "FParsec/lib/portable-net45+netcore45+wpa81+wp8/FParsec.dll"
#r "FSharp.Collections.ParallelSeq/lib/net40/FSharp.Collections.ParallelSeq.dll"

open System.Text.RegularExpressions
open FsVerbalExpressions
open System
open FParsec
open System.IO
open FSharp.Collections.ParallelSeq

let test p str = 
    match run p str with
    | Success(result, _, _) -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

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
let sample2 = """
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

"The Rishi replied, 'The Purana, first promulgated by the great Rishi
Dwaipayana, and which after having been heard both by the gods and the
Brahmarshis was highly esteemed, being the most eminent narrative that
exists, diversified both in diction and division, possessing subtile
meanings logically combined, and gleaned from the Vedas, is a sacred
work. Composed in elegant language, it includeth the subjects of other
books. It is elucidated by other Shastras, and comprehendeth the sense of
the four Vedas. We are desirous of hearing that history also called
Bharata, the holy composition of the wonderful Vyasa, which dispelleth
the fear of evil, just as it was cheerfully recited by the Rishi
Vaisampayana, under the direction of Dwaipayana himself, at the
snake-sacrifice of Raja Janamejaya?'

"Sauti then said, 'Having bowed down to the primordial being Isana, to
whom multitudes make offerings, and who is adored by the multitude; who
is the true incorruptible one, Brahma, perceptible, imperceptible,
eternal; who is both a non-existing and an existing-non-existing being;
who is the universe and also distinct from the existing and non-existing
universe; who is the creator of high and low; the ancient, exalted,
inexhaustible one; who is Vishnu, beneficent and the beneficence itself,
worthy of all preference, pure and immaculate; who is Hari, the ruler of
the faculties, the guide of all things moveable and immoveable; I will
declare the sacred thoughts of the illustrious sage Vyasa, of marvellous
deeds and worshipped here by all. Some bards have already published this
history, some are now teaching it, and others, in like manner, will
hereafter promulgate it upon the earth. It is a great source of
knowledge, established throughout the three regions of the world. It is
possessed by the twice-born both in detailed and compendious forms. It is
the delight of the learned for being embellished with elegant
expressions, conversations human and divine, and a variety of poetical
measures.

In this world, when it was destitute of brightness and light, and
enveloped all around in total darkness, there came into being, as the
primal cause of creation, a mighty egg, the one inexhaustible seed of all
created beings. It is called Mahadivya, and was formed at the beginning
of the Yuga, in which we are told, was the true light Brahma, the eternal
one, the wonderful and inconceivable being present alike in all places;
the invisible and subtile cause, whose nature partaketh of entity and
non-entity. From this egg came out the lord Pitamaha Brahma, the one only
Prajapati; with Suraguru and Sthanu. Then appeared the twenty-one
Prajapatis, viz., Manu, Vasishtha and Parameshthi; ten Prachetas, Daksha,
and the seven sons of Daksha. Then appeared the man of inconceivable
nature whom all the Rishis know and so the Viswe-devas, the Adityas, the
Vasus, and the twin Aswins; the Yakshas, the Sadhyas, the Pisachas, the
Guhyakas, and the Pitris. After these were produced the wise and most
holy Brahmarshis, and the numerous Rajarshis distinguished by every noble
quality. So the water, the heavens, the earth, the air, the sky, the
points of the heavens, the years, the seasons, the months, the
fortnights, called Pakshas, with day and night in due succession. And
thus were produced all things which are known to mankind.

And what is seen in the universe, whether animate or inanimate, of
created things, will at the end of the world, and after the expiration of
the Yuga, be again confounded. And, at the commencement of other Yugas,
all things will be renovated, and, like the various fruits of the earth,
succeed each other in the due order of their seasons. Thus continueth
perpetually to revolve in the world, without beginning and without end,
this wheel which causeth the destruction of all things.
"""

sample.Replace("\"", "")

let simpleEmail = "contact@kunjan.in"

test pfloat "1.15"

let str s = pstring s
let emailtest = pipe2 pfloat (str "@" >>. pfloat) (fun x y -> (x, y))

test emailtest "3@5"
sample.Replace("\"", "").Replace("\'", "")
|> fun x -> Regex.Split(x, "said")
|> Array.toList
|> fun x -> 
    match x with
    | head :: tail -> 
        (head.Trim(), 
         match tail with
         | head :: tail -> head.Trim()
         | _ -> "")
    | _ -> ("", "")
Regex.Split(sample2, "\n\n") |> Array.map (fun x -> 
                                    x.Replace("\"", "").Replace("\'", "")
                                    |> fun x -> Regex.Split(x, "said")
                                    |> Array.toList
                                    |> fun x -> 
                                        match x with
                                        | head :: tail -> 
                                            (head.Trim(), 
                                             match tail with
                                             | head :: tail -> ""
                                             | _ -> "")
                                        | _ -> ("", ""))

(* 
    Filtering name based on capital later. Thanks to Eve. 
*)
let checkisName (str : string) = isUpper str.[0]
//should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
let cleanedUpSample (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")
//remove roman numbers 
let isRoman str = Regex.IsMatch(str, "^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")
let filterObvious str = str <> "I" && str <> "A" && str <> "An" && str <> "The"
let filterSalutation str = str <> "O"
let combineFilter str = 
    (not <| String.IsNullOrWhiteSpace str) && (not <| isRoman str) && (filterObvious str) && (checkisName str)
    && filterSalutation str
 
let processStr (str : string) = 
    str.Split(' ')
    |> Array.map (fun x -> x.Trim())

let allProbableNames (str : string) =
    // Looking up lower case words was very slow 
    // because it was searching through the full text
    // Changed it into a Set - constant lookup
    let processed = processStr str |> set 
    processStr str
    |> Array.filter combineFilter
    |> Array.filter (fun x -> not (processed.Contains(x.ToLower())))

let names = cleanedUpSample >> allProbableNames //this would have all name appearance in sample. 
let uniqueNames str = names str |> Array.countBy id

//big volume1 data. 
let volume1 = 
    Path.Combine(__SOURCE_DIRECTORY__, "..", "txt_data/volume1.txt")
    |> File.ReadAllLines
    |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
    |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
    |> String.concat "\n"

let printSeq strs = strs |> Array.iter (fun x -> printfn "%A" x)

printSeq <| (uniqueNames volume1 |> Array.take 100)
let nameCandidates = uniqueNames volume1

// Look at top names
nameCandidates
|> Array.sortByDescending snd
|> Array.take 50
|> printSeq
