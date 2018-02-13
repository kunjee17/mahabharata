//Parse name which comes before said
// open VerbalExpression
//can't make it work with string if I am passing "a said b" then it should return (a,b)
//fall back to string split.
//TODO use regex or FParsec for better performace
#I "../packages"
#r "FsVerbalExpressions/lib/net461/FsVerbalExpressions.dll"
#r "FParsec/lib/portable-net45+netcore45+wpa81+wp8/FParsecCS.dll"
#r "FParsec/lib/portable-net45+netcore45+wpa81+wp8/FParsec.dll"
#r "FSharp.Collections.ParallelSeq/lib/net40/FSharp.Collections.ParallelSeq.dll"

open System.Text.RegularExpressions
open System

open System.IO


(*
  Filtering name based on capital later. Thanks to Eve.
*)
let checkisName (str : string) = Char.IsUpper str.[0]
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

let printSeq (strs:string []) = strs |> Array.iter (fun x -> printfn "%A" x)

// printSeq <| (uniqueNames volume1 |> Array.take 100)
let nameCandidates = uniqueNames volume1

// Look at top names
let nameCandidatesArr =
    nameCandidates
    |> Array.sortByDescending snd

let namesOnly = nameCandidatesArr |> Array.map fst

let top50 =
    nameCandidatesArr
    |> Array.take 50

top50 |> Array.map (fun (a,_) -> a) |> printSeq

