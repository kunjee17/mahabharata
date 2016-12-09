(**
Lesson 1 - Basic understanding of NLP. Getting started with term,document frequency for term and Inverse document frequency for term.
*)
#I "../packages"
#r "Microsoft.FsharpLu.Json/lib/net452/Microsoft.FsharpLu.Json.dll"
#r "Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "FSharp.Data/lib/net40/FSharp.Data.dll"

open System.IO
open System
open System.Text.RegularExpressions
open Newtonsoft.Json
open Microsoft.FSharpLu.Json
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.SparseMatrix
open FSharp.Data

//Basic_Emotions_(size_is_proportional_to_number_of__data.csv
let EmotionalPath = Path.Combine(__SOURCE_DIRECTORY__, "..","data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv")
type EmotionCsv = CsvProvider< "../data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv">



let EmotionData = EmotionCsv.Load("../data/Basic_Emotions_(size_is_proportional_to_number_of__data.csv")

type EmotionInNumber = {
    Anger:int
    Anticipation:int
    Disgust:int
    // Emotion:int
    Fear:int
    Joy:int
    Negative:int
    Positive:int
    Sadness:int
    Surprise:int
    Trust:int
    Word: string
}

let JsonToFile (path:string)(text : string) =
    use writer = new StreamWriter(path)
    writer.WriteLine (text)
let stringToNum s = if String.IsNullOrEmpty(s) then 0 else 1

let emotionCalculate (anger,anticipation, disgust,emotion,fear, joy, negative, positive, sadness, surprise, trust, word) =     
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
        | "anger" -> if a.Anger = 0 then {a with Anger = 1} else a
        | "anticip" -> if a.Anticipation = 0 then {a with Anticipation = 1} else a 
        | "disgust" -> if a.Disgust = 0 then {a with Disgust = 1 } else a
        | "fear" -> if a.Fear = 0 then {a with Fear = 1} else a
        | "joy" -> if a.Joy = 0 then {a with Joy = 1} else a
        | "negative" -> if a.Negative = 0 then {a with Negative = 1} else a
        | "positive" -> if a.Positive = 0 then {a with Positive = 1} else a
        | "sadness" -> if a.Sadness = 0 then {a with Sadness = 1} else a
        | "surprise" -> if a.Surprise = 0 then {a with Surprise = 1} else a
        | "trust" -> if a.Trust = 0 then {a with Trust = 1} else a
        | _ -> a 
    

let allEmotionsInNumber = EmotionData.Rows |> Seq.map (fun row -> emotionCalculate (
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
let emotionWordsSet = allEmotionsInNumber |> Seq.map (fun row -> row.Word) |> set
// |> Seq.map (fun (nor,anger,anticipation,disgust,emotion,fear,joy,negative,positive,sadness,trust,word) -> ignore)


type DocItem = {
    Term : string
    Freq : int
}

type Doc = {
    Bookno : string
    DocItems : DocItem []
}

let zeroEmotion = {
        Anger = 0
        Anticipation = 0
        Disgust =0
        // Emotion = 0
        Fear = 0
        Joy = 0
        Negative = 0
        Positive = 0
        Sadness = 0
        Surprise = 0
        Trust = 0
        Word = "book0"
    }

let bookSum bookname a b =
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
        Word = bookname
    }


type Book(bookno:string, bookname:string) =

    //should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
    let removeSpecialChars (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")

    member x.BookNo = bookno + "-" + bookname
    member x.BookText =
                Path.Combine(__SOURCE_DIRECTORY__, "..", "books/"+ bookno + ".txt")
                |> File.ReadAllLines

    member x.TermFrequency (input:string) =
            input
            // |> String.concat " "
            |> (fun x -> x.Split ' ')
            |> Array.map (removeSpecialChars >> (fun x -> x.Trim()))
            |> Array.filter (fun x -> x <> "")
            |> Array.countBy id
    member x.BookTermGroup =
            x.TermFrequency (x.BookText |> String.concat " ")

    member x.BookUniqueTerms =
        x.BookTermGroup
            |> Array.map (fun (x,_)-> x)
            |> set

    member x.EmotionalIndex =
        let commonEmotions = x.BookUniqueTerms |> Set.intersect emotionWordsSet
        let commonEmotionsCount = commonEmotions.Count
        let commonEmotionsInNumber = allEmotionsInNumber |> Seq.filter (fun x -> commonEmotions.Contains x.Word) |> Seq.toArray
        let r = commonEmotionsInNumber |> Array.fold (bookSum x.BookNo) zeroEmotion
        { r with
            Anger = (r.Anger * 100/commonEmotionsCount)
            Anticipation = (r.Anticipation * 100/commonEmotionsCount)
            Disgust =(r.Disgust * 100/commonEmotionsCount)
            // Emotion = (r.Emotion * 100/commonEmotionsCount)
            Fear = (r.Fear * 100/commonEmotionsCount)
            Joy = (r.Joy * 100/commonEmotionsCount)
            Negative = (r.Negative * 100/commonEmotionsCount)
            Positive = (r.Positive * 100/commonEmotionsCount)
            Sadness = (r.Sadness * 100/commonEmotionsCount)
            Surprise = (r.Surprise * 100/commonEmotionsCount)
            Trust = (r.Trust * 100/commonEmotionsCount)
        }


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
        yield Book(n,m)
|]

let book0 = books.[0]

let allBookEmotionalIndex = books |> Array.map (fun x -> x.EmotionalIndex)

let emotionalJsonData = Compact.serialize allBookEmotionalIndex

let emotionalPath = Path.Combine(__SOURCE_DIRECTORY__, "..", "docs/js/" + "emotional" + ".json")
JsonToFile emotionalPath emotionalJsonData



let posnegWordList =
    Path.Combine(__SOURCE_DIRECTORY__, "..", "data/AFINN/"+ "AFINN-111" + ".txt")
    |> File.ReadAllLines
    |> Array.map (fun x ->
                    x.Split '\t' |> (fun b -> b.[0],b.[1])
                    )






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
    {Bookno = bookno; DocItems = docItems}


let uniqueTermsforAllBooksAsDoc = booknos |> Array.map getUniqueTermsByFrequencyForBook

let uniqueTermsforAllBooks = books |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (fun acc elem -> acc + elem) Set.empty

let invertedIndex (term:string) =
    let n = books |> Array.filter (fun x -> x.BookUniqueTerms.Contains term) |> Array.length |> float
    let m = books |> Array.length |> float
    log (m/n)


let invertedIndexTerms =
    uniqueTermsforAllBooks
        |> Set.map (fun x -> (x,invertedIndex x))

// let terms =
//     Compact.serialize uniqueTermsforAllBooksAsDoc



// let jsonpath = Path.Combine(__SOURCE_DIRECTORY__, "..", "docs/js/" + "terms" + ".js")

// JsonToFile jsonpath terms



