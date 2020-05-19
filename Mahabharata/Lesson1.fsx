(**
Lesson 1 - Basic understanding of NLP. Getting started with term,document frequency for term and Inverse document frequency for term.
*)
#I "../packages"
#r "Microsoft.FsharpLu.Json/lib/net452/Microsoft.FsharpLu.Json.dll"
#r "Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "MathNet.Numerics/lib/net461/MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp/lib/net45/MathNet.Numerics.FSharp.dll"
#r "FSharp.Data/lib/net45/FSharp.Data.dll"

open System.IO
open System
open System.Text.RegularExpressions
open Newtonsoft.Json
open Microsoft.FSharpLu.Json
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.SparseMatrix
open FSharp.Data

[<AutoOpen>]
module FNLP =

    let stringToNum s = if String.IsNullOrEmpty(s) then 0. else 1.

    //should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
    let removeSpecialChars (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")


    let Terms (input:string)=
            input
            |> (fun x -> x.Split ' ')
            |> Array.map (removeSpecialChars >> (fun x -> x.Trim()))
            |> Array.filter (fun x -> x <> "")

    let UniqueTermsWithFrequency (input:string[])=
            input
            |> Array.countBy id


    //Basic_Emotions_(size_is_proportional_to_number_of__data.csv
    let sentimentCSVPath = Path.Combine(__SOURCE_DIRECTORY__, "..","data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv")
    type SentimentCsv = CsvProvider< "../data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv">

    let SentimentData = SentimentCsv.Load("../data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv")


    type SentimentInNumber = {
        Anger:float
        Anticipation:float
        Disgust:float
        Fear:float
        Joy:float
        Negative:float
        Positive:float
        Sadness:float
        Surprise:float
        Trust:float
        Word: string
    }

    let ZeroSentiment = {
        Anger = 0.
        Anticipation = 0.
        Disgust =0.
        Fear = 0.
        Joy = 0.
        Negative = 0.
        Positive = 0.
        Sadness = 0.
        Surprise = 0.
        Trust = 0.
        Word = String.Empty
    }

    //Active Pattern for Sentiments
    let (|Anger|_|) input =
        if input = "anger" then Some Anger else None
    let (|Anticipation|_|) input =
        if input = "anticip" then Some Anticipation else None
    let (|Disgust|_|) input =
        if input = "disgust" then Some Disgust else None
    let (|Fear|_|) input =
        if input = "fear" then Some Fear else None
    let (|Joy|_|) input =
        if input = "joy" then Some Joy else None
    let (|Negative|_|) input =
        if input = "negative" then Some Negative else None
    let (|Positive|_|) input =
        if input = "positive" then Some Positive else None
    let (|Sadness|_|) input =
        if input = "sadness" then Some Sadness else None
    let (|Surprise|_|) input =
        if input = "surprise" then Some Surprise else None
    let (|Trust|_|) input =
        if input = "trust" then Some Trust else None



    let sentimentCalculate (anger,anticipation, disgust,emotion,fear, joy, negative, positive, sadness, surprise, trust, word) =
        let a = {
            Anger = stringToNum anger
            Anticipation = stringToNum anticipation
            Disgust = stringToNum disgust
            Fear = stringToNum fear
            Joy = stringToNum joy
            Negative = stringToNum negative
            Positive = stringToNum positive
            Sadness = stringToNum sadness
            Surprise = stringToNum surprise
            Trust = stringToNum trust
            Word = word
        }

        match emotion with
        | Anger -> if a.Anger = 0. then {a with Anger = 1.} else a
        | Anticipation -> if a.Anticipation = 0. then {a with Anticipation = 1.} else a
        | Disgust -> if a.Disgust = 0. then {a with Disgust = 1. } else a
        | Fear -> if a.Fear = 0. then {a with Fear = 1.} else a
        | Joy -> if a.Joy = 0. then {a with Joy = 1.} else a
        | Negative -> if a.Negative = 0. then {a with Negative = 1.} else a
        | Positive -> if a.Positive = 0. then {a with Positive = 1.} else a
        | Sadness -> if a.Sadness = 0. then {a with Sadness = 1.} else a
        | Surprise -> if a.Surprise = 0. then {a with Surprise = 1.} else a
        | Trust -> if a.Trust = 0. then {a with Trust = 1.} else a
        | _ -> a

    let allSentimentsInNumber =
        SentimentData.Rows
        |> Seq.map (fun row ->
            sentimentCalculate (
                    row.Anger,
                    row.Anticipation,
                    row.Disgust,
                    row.Emotion,
                    row.Fear,
                    row.Joy,
                    row.Negative,
                    row.Positive,
                    row.Sadness,
                    row.Surprise,
                    row.Trust,
                    row.Word
                    ))
    let SentimentWordsSet = allSentimentsInNumber |> Seq.map (fun row -> row.Word) |> set

    let SentimentSum word a b =
        {
            Anger = a.Anger + b.Anger
            Anticipation = a.Anticipation + b.Anticipation
            Disgust = a.Disgust + b.Disgust
            // Emotion = a.Emotion + b.Emotion
            Fear = a.Fear + b.Fear
            Joy = a.Joy + b.Joy
            Negative = a.Negative + b.Negative
            Positive = a.Positive + b.Positive
            Sadness = a.Sadness + b.Sadness
            Surprise = a.Surprise + b.Surprise
            Trust = a.Trust + b.Trust
            Word = word
        }


    type Word = {
        Term :string
        Rating : int
    }

    let WordList =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "data/AFINN/"+ "AFINN-111" + ".txt")
        |> File.ReadAllLines
        |> Array.map (fun x ->
                        x.Split '\t' |> (fun b -> {Term = b.[0]; Rating = System.Int32.Parse b.[1]})
                        )


[<AutoOpen>]
module Utility =
    let JsonDropPath name =
        Path.Combine (__SOURCE_DIRECTORY__, "..", "docs/js/" + name + ".json")
    let dataToJSONFile (fileName : string)(data :'a) =
        let path = JsonDropPath fileName
        use writer = new StreamWriter(path)
        let txt = data |> Compact.serialize
        writer.WriteLine (txt)




module Book =
    type Book = {
        Name : string
        Text : string
        UniqueTerms : Set<string>
        Terms : string []
        UniqueTermsWithFrequency : (string * int) []
        SentimentIndex : SentimentInNumber
        WordsRating : Word []
    }

    let create (bookname:string) (booktext :string) =
        let terms = Terms booktext
        let termsCount = terms.Length |> float
        let termsWithFrequency = UniqueTermsWithFrequency terms
        let uniqueTerms = termsWithFrequency |> Array.map (fun (x,_) -> x)

        {
            Name = bookname
            Text = booktext
            UniqueTerms = uniqueTerms |> set
            Terms = terms
            UniqueTermsWithFrequency = termsWithFrequency
            SentimentIndex =
                let commonEmotions = uniqueTerms |> set |> Set.intersect SentimentWordsSet
                let commonEmotionsCount = termsWithFrequency
                                            |> Array.filter(fun (x,_) -> commonEmotions.Contains x)
                                            |> Array.map (fun (_,y) -> y) |> Array.sum |> float
                let commonEmotionsInNumber = allSentimentsInNumber |> Seq.filter (fun x -> commonEmotions.Contains x.Word) |> Seq.toArray
                let r = commonEmotionsInNumber |> Array.fold (SentimentSum bookname) ZeroSentiment

                { r with
                    Anger = (r.Anger/commonEmotionsCount) * 100.
                    Anticipation = (r.Anticipation/commonEmotionsCount) * 100.
                    Disgust =(r.Disgust/commonEmotionsCount) * 100.
                    // Emotion = (r.Emotion)
                    Fear = (r.Fear/commonEmotionsCount) * 100.
                    Joy = (r.Joy/commonEmotionsCount) * 100.
                    Negative = (r.Negative/commonEmotionsCount) * 100.
                    Positive = (r.Positive/commonEmotionsCount) * 100.
                    Sadness = (r.Sadness/commonEmotionsCount) * 100.
                    Surprise = (r.Surprise/commonEmotionsCount) * 100.
                    Trust = (r.Trust/commonEmotionsCount) * 100.
                }
            WordsRating =
                let commonWords =
                    WordList
                    |> Array.map (fun x -> x.Term)
                    |> set
                    |> Set.intersect (uniqueTerms |> set)
                WordList
                |> Array.filter (fun a -> commonWords.Contains a.Term)
        }


//All books of Mahabharata starts procession from here.

//Book Nos as per filnames
let booknos = [|
                "01";
                "02";
                "03";
                "04";
                "05";
                "06";
                "07";
                "08";
                "09";
                "10";
                "11";
                "12";
                "13";
                "14";
                "15";
                "16";
                "17";
                "18"
                |]

//Book names
let booknames = [|
                "Adi Parva";
                "Sabha Parva";
                "Vana Parva";
                "Virata Parva";
                "Udyoga Parva";
                "Bhishma Parva";
                "Drona Parva";
                "Karna Parva";
                "Shalya Parva";
                "Sauptika Parva";
                "Stri Parva";
                "Shanti Parva";
                "Anushasana Parva";
                "Ashvamedhika Parva";
                "Ashramavasika Parva";
                "Mausala Parva";
                "Mahaprasthanika Parva";
                "Svargarohana Parva"
                |]
let books = [|
    for n,m in Array.zip booknos booknames do
        let txt = Path.Combine(__SOURCE_DIRECTORY__, "..", "books/"+ n + ".txt")
                    |> File.ReadAllLines |> String.concat " "
        yield Book.create (n+"-"+m) txt
|]

// let book0 = books.[0]

// books |> Array.map (fun a -> (a.Name, a.WordsRating))

let allBookEmotionalIndex = books |> Array.map (fun x -> x.SentimentIndex)


dataToJSONFile "emotional" allBookEmotionalIndex



(*
    Anger,Anticipation,Disgus,Fear,Joy,Negative,Positive,Sadness,Surprise,Trust,
*)

dataToJSONFile "anger" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Anger)))
dataToJSONFile "anticipation" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Anticipation)))
dataToJSONFile "disgus" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Disgust)))
dataToJSONFile "fear" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Fear)))
dataToJSONFile "joy" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Joy)))
dataToJSONFile "negative" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Negative)))
dataToJSONFile "positive" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Positive)))
dataToJSONFile "sadness" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Sadness)))
dataToJSONFile "surprise" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Surprise)))
dataToJSONFile "trust" (books |> Array.map (fun x -> (x.Name, x.SentimentIndex.Trust)))

//Terms and UniqueTerms

dataToJSONFile "terms" (books |> Array.map (fun x -> (x.Name, x.Terms.Length)))
dataToJSONFile "uniqueterms" (books |> Array.map (fun x -> (x.Name, x.UniqueTerms.Count)))
dataToJSONFile "utbyterms" (books |> Array.map (fun x -> (x.Name,
                                                                    ((x.UniqueTerms.Count |> float) / (x.Terms.Length |> float)) * 100.
                                                    )))

let termsInWordRating (book:Book.Book)=
    let currentTermsInWordRating =
        book.UniqueTerms
        |> Set.intersect (book.WordsRating |> Array.map (fun x -> x.Term) |> set)

    let termsWithFrequency =
        book.UniqueTermsWithFrequency
                        |> Array.filter (fun (x,_) -> currentTermsInWordRating.Contains x )
                        |> Array.sortBy (fun (_,y) -> -y)
                        |> Array.take 51
                        |> Array.map (fun (x,y)->
                                        let term = book.WordsRating |> Array.find (fun a -> a.Term = x)
                                        (x, y * term.Rating)
                                   )
                        |> Array.sortBy (fun (_,y)-> -y)
    let bookfileName = book.Name.ToLower().Replace(" ", "-") + "-rating"
    dataToJSONFile bookfileName termsWithFrequency

books |> Array.iter termsInWordRating


(**
    Inverted Index. WIP.

    let (m : Matrix<float>) =
    let allbookTerms = books
                        |> Array.map (fun x -> x.BookUniqueTerms)
                        |> Array.fold (+) Set.empty
    let sm = SparseMatrix.zero books.Length allbookTerms.Count
    let book0 = books.[0]
    allbookTerms
        |> Set.toArray
        |> Array.iteri (fun i x  ->
                            if book0.BookUniqueTerms.Contains x then  sm.[0,i]<- 1.0
                        )
    sm

    let getUniqueTermsByFrequencyForBook (bookno : string) : Doc =
        let bookToProcess = books |> Array.find (fun x -> x.BookNo = bookno)
        let allOtherBooks = books |> Array.filter (fun x -> x.BookNo <> bookno)

        let allBookTerms = allOtherBooks |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (+) Set.empty

        let termsSpecifictoBook = bookToProcess.BookUniqueTerms - allBookTerms
        let docItems: DocItem [] = bookToProcess.BookTermGroup
                                    |> Array.filter (fun (x,_) -> termsSpecifictoBook.Contains x)
                                    |> Array.map (fun (x,y)-> { Term = x; Freq = y})
        {BookName = bookno; DocItems = docItems}


    let uniqueTermsforAllBooksAsDoc = booknos |> Array.map getUniqueTermsByFrequencyForBook

    let uniqueTermsforAllBooks = books |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (fun acc elem -> acc + elem) Set.empty

    let invertedIndex (term:string) =
        let n = books |> Array.filter (fun x -> x.BookUniqueTerms.Contains term) |> Array.length |> float
        let m = books |> Array.length |> float
        log (m/n)


    let invertedIndexTerms =
        uniqueTermsforAllBooks
            |> Set.map (fun x -> (x,invertedIndex x))

    type DocItem = {
        Term : string
        Freq : int
    }

    type Doc = {
        BookName : string
        DocItems : DocItem []
    }

*)

