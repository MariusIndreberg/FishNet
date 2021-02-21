module monoTest.Triangle
open Microsoft.Xna.Framework
open System
open Microsoft.Xna.Framework.Graphics

type Node = {
    Pos : Vector2
}

type Elem = {
    Nodes : Node*Node*Node
}
module Elem = 
    let New node1 node2 node3 = 
        {
            Nodes = (node1, node2, node3)
        }   
    
    let InTriangle (elem : Elem) (p : Vector2) = 
        let sign (p1 : Vector2) (p2 : Vector2) (p3 : Vector2) = 
            (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y) 
        let p1,p2,p3 = elem.Nodes
        let d1 = sign p p1.Pos p2.Pos
        let d2 = sign p p2.Pos p3.Pos 
        let d3 = sign p p3.Pos p1.Pos
        not (((d1 < 0f) || (d2 < 0f) || (d3 < 0f)) && ((d1 > 0f) || (d2 > 0f) || (d3 > 0f)))