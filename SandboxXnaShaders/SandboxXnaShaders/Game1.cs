using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SandboxXnaShaders
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Effect effect;

        Matrix world, view, projection;
        Vector3 cameraPosition;

        #endregion Fields

        #region Initialization

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Models/Object");
            effect = Content.Load<Effect>("Effects/Ambient");

            CreateWorld();
        }

        private void CreateWorld()
        {
            world = Matrix.Identity;

            cameraPosition = new Vector3(3, 3, 10);
            Vector3 lookAt = Vector3.Zero;
            view = Matrix.CreateLookAt(cameraPosition, lookAt, Vector3.Up);

            projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4, GraphicsDevice.Viewport.AspectRatio, 1, 1000);
        }

        #endregion Initialization

        #region Dispose

        protected override void UnloadContent()
        {
        }

        #endregion Dispose

        #region Loop

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DrawModel(model, world, view, projection);
            DrawModelWithEffect(model, world, view, projection);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        #endregion Loop
    }
}
