#r "../node_modules/fable-core/Fable.Core.dll"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser

module JQuery = 

    [<Import("*","jquery")>]
    type JQuery = 
        abstract appendTo: JQuery -> JQuery
        abstract get: int -> JQuery
        abstract remove: unit -> JQuery
        abstract on: string -> (obj -> unit) -> JQuery
        abstract text: unit -> string
        abstract css: string -> string -> JQuery

    [<Emit("jQuery($0)")>]
    let jquery (selector: obj) : JQuery = failwith "JS only"
    
    let appendTo target (selector: JQuery) = selector.appendTo target
    let remove (selector: JQuery) = selector.remove ()
    let get (index: int) (selector: JQuery) = selector.get index
    let on event handler (selector: JQuery) = selector.on event handler
    
    [<Emit("$1.attr($0)")>]
    let getAttr attr (selector: JQuery): string = failwith "JS only"
    [<Emit("$2.attr($0, $1)")>]
    let setAttr attr value (selector: JQuery): string = failwith "JS only"
    let text (selector: JQuery): string = selector.text()
    let css prop value (selector: JQuery) = selector.css prop value


module Utils =
    let private random = new Random()
    let shuffle lst =
        lst
        |> List.map (fun i -> i, random.Next())
        |> List.sortBy snd
        |> List.map fst


module GameLogic = 
    open Utils
    let generateNumbers max = [1..max] |> shuffle


module View = 
    open JQuery
    open GameLogic

    let mutable gamestate = 1

    let container = jquery ".game-field"
    let progress = jquery ".progress"

    let createResultNum num = 
        sprintf "<div class=\"result-num\">%d</div>" num
        |> jquery

    let clickHandler e = 
        let target = jquery e?target
        let num = target |> text |> int
        if gamestate = num then
            num |> createResultNum |> appendTo progress |> ignore
            target |> css "visibility" "hidden" |> ignore
            gamestate <- gamestate + 1
            ()

    let createBlock num = 
        sprintf "<div class=\"block\">%d<div>" num
        |> jquery
        |> on "click" clickHandler

    let start () = 
        generateNumbers 42 
        |> List.map createBlock
        |> List.iter (fun block -> block.appendTo container |> ignore)

View.start ()