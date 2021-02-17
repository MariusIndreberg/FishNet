module monoTest.FishNet
open Microsoft.Xna.Framework
open System
open Microsoft.Xna.Framework.Graphics

[<Literal>]
let BoundX = 800 

[<Literal>]
let BoundY = 600

[<Struct>]
type Particle = {
    Uuid        : Guid
    Pos         : Vector2 
    Velocity    : Vector3
    Acc         : Vector3
    Z           : int 
}
module Particle = 
    let New pos speed = {
        Uuid        = Guid.NewGuid()
        Pos         = pos 
        Velocity    = Vector3.Zero 
        Acc         = speed
        Z           = 0
    }

    let private checkCollisionBound particle = 
        particle.Pos.X + particle.Velocity.X < 0f || 
        particle.Pos.X + particle.Velocity.X > float32 BoundX ||
        particle.Pos.Y + particle.Velocity.Y < 0f ||
        particle.Pos.Y + particle.Velocity.Y > float32 BoundY 
    
    let private reversePosition particle = 
        let vel = particle.Velocity * -1f
        let pos = Vector2(particle.Pos.X + vel.X, particle.Pos.Y + vel.Y)
        { particle with Pos = pos; Velocity = vel; Acc = particle.Acc * -1f }        

    let private updatePosition particle =   
        let vel = particle.Velocity + particle.Acc   
        vel.Normalize()
        let pos' = Vector2(particle.Pos.X + vel.X, particle.Pos.Y + vel.Y)
        { particle with  Pos = pos'; Velocity = vel }  

    let private advectOceanCurrent particle = 
        if checkCollisionBound particle then 
            reversePosition particle 
        else 
            updatePosition particle

    let private advectWind particle = 
        particle

    let Update particle =
        advectOceanCurrent particle 
            

type Zone = {
    Draft : Vector3
    Bound : Rectangle
}
module Zone = 
    let New bound dr = {
        Draft = dr 
        Bound = bound
    }
    let private inTriangle ((p1, p2, p3) : Vector2*Vector2*Vector2) (p : Vector2) = 
        let sign (p1 : Vector2) (p2 : Vector2) (p3 : Vector2) = 
            (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y) 

        let d1 = sign p p1 p2
        let d2 = sign p p2 p3 
        let d3 = sign p p3 p1
        not (((d1 < 0f) || (d2 < 0f) || (d3 < 0f)) && ((d1 > 0f) || (d2 > 0f) || (d3 > 0f)))


    let Update (p : Particle []) (z : Zone) = 
        p
        |> Array.Parallel.map (fun particle -> 
            if z.Bound.Contains particle.Pos then 
                let acc' = z.Draft + particle.Acc  
                acc'.Normalize()
                { particle with Acc = acc' }
            else particle )

type Simulation = {
    Particles   : Particle []
    Zones       : Zone []
    InputData   : Map<int, Zone []>
    CurrentStep : int 
}
module Simulation = 
    let Init (n : int) = 
        let r = Random() 
        {
            CurrentStep = 0
            InputData   = Map.empty
            Particles   = Array.init n (fun x -> 
                let pos = Vector2((float32)(r.Next() % BoundX), (float32)(r.Next() % BoundY))
                let vel = Vector3((float32)(r.Next()), (float32)(r.Next()),(float32)(r.Next()))
                vel.Normalize()
                Particle.New pos vel 
            )
            Zones       = [|
                Zone.New (Rectangle(0,0,400,600)) (Vector3(0.01f, 0.01f, -0.01f))
                Zone.New (Rectangle(400,0,400,600)) (Vector3(-0.01f, -0.01f, 0.01f))
            |]
        }

    let Update (s : Simulation) (gt : GameTime ) =
        s.Zones 
        |> Array.Parallel.collect (fun zone -> 
            Zone.Update s.Particles zone )
        |> Array.distinctBy (fun p -> p.Uuid)
        //|> fun x -> printfn "%A" x.Length; x
        |> Array.Parallel.map Particle.Update
        |> fun x -> { s with Particles = x }

    let Draw (s : Simulation) (tex : Texture2D) (sb : SpriteBatch) (gameTime : GameTime) = 
        s.Particles
        |> Array.iter (fun p -> sb.Draw(tex, p.Pos, Color.White) )