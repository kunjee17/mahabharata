#load "NameFinder.fs"
open System
open NameFinder

// Filter character names

let nameSet = set namesOnly

// Split text into individual words
let allWords = 
    volume.Split(" .,()!?-\n\t".ToCharArray())
    |> Array.map (fun s -> s.Trim())
    |> Array.filter ((<>) "")

// For each name, find the next word in the text
let nameWithNextWord = 
    allWords
    |> Array.mapi (fun i word -> 
        if nameSet.Contains word then Some(word, allWords.[i+1])
        else None)
    |> Array.choose id

// Look at the next words to find those that correspond
// to some form of action that a character would do
let nextWords = 
    nameWithNextWord 
    |> Array.map snd 
    |> Array.countBy id 
    |> Array.sortByDescending snd
nextWords.[0..200]
|> Array.iter (printfn "%A")

// Very heuristically and approximately extracting names that are 
// immediately followed by some word that implies the name is actually
// a name of a person
// This should keep almost all main characters
let maybeIsCharacter w = 
    w = "asked" || w = "said" || w = "replied" 
    || w = "answered" || w = "married" || w = "went"
    || w = "who" || w = "himself" || w = "herself" 
    || w = "themselves" || w = "sent" || w = "alone"
    || w = "called" || w = "saying" || w = "came"
    || w = "begat" || w = "addressed" || w = "hearing"
    || w = "princes" || w = "saw" || w = "seeing" 
    || w = "prince" || w = "took" || w = "called"
    || w = "king" || w = "gave" || w = "taking"
    || w = "addressing" || w = "spoke" || w = "born"
    || w = "performed"

// These should be real names of active characters
let surelyNames =
    nameWithNextWord 
    |> Array.filter (fun (_, w) -> maybeIsCharacter w)
    |> Array.map fst
    |> set

// How many names are there?
surelyNames.Count
namesOnly.Length

// These should be names of places etc., or names of relatively minor
// characters that are not very active
let maybeNotNames =
    nameWithNextWord
    |> Array.filter (fun (n, w) -> 
        (not (maybeIsCharacter w)) && (not (surelyNames.Contains n)))
    |> Array.map fst
    |> Array.distinct

maybeNotNames |> Array.iter (fun x -> printfn "%A" x)

maybeNotNames |> Array.findIndex (fun x -> x = "Duhsasana")

// Saraswati is a goddess & a river?

// TODO: 
// - If there are two potential names immediately after each other,
//   they might be a name of a single character. Currently the first of the
//   two names is filtered out
// - Maybe extend the context - if a name is preceded of followed by 
//   "son of" or "daughter of" then it's also a character name

