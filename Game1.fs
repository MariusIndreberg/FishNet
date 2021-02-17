namespace monoTest
open FishNet
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
type Game1 () as this =
    inherit Game()
    let mutable simulation = Simulation.Init 100000
    //let mutable fluid = Fluid.New 256 0.2 0.0 0.000001
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<_>
    let mutable pixel = Unchecked.defaultof<_>
    do
        this.Content.RootDirectory <- "Content"
        this.IsMouseVisible <- true

    override this.Initialize() =
        // TODO: Add your initialization logic here
        pixel <- new Texture2D(this.GraphicsDevice, 1, 1);
        pixel.SetData([|Color.White|]);
        base.Initialize()
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        // TODO: use this.Content to load your game content here
 
    override this.Update (gameTime) =
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        then this.Exit();
        simulation <- Simulation.Update simulation gameTime
        // TODO: Add your update logic here
        //fluid.Update 
        base.Update(gameTime)
 
    override this.Draw (gameTime) =
        this.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin()
        Simulation.Draw simulation pixel spriteBatch gameTime
        //fluid.Draw pixel spriteBatch
        spriteBatch.End()
        base.Draw(gameTime)

