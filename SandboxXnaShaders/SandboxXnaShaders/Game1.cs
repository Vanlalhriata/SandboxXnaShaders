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
        Texture2D texture;
        Texture2D normalMap;

        Matrix world, view, projection;
        Vector3 cameraPosition;
        Vector3 lookAt;
        float angle = 0;

        Vector3 viewVector;

        #endregion Fields

        #region Initialization

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = true;
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
            effect = Content.Load<Effect>("Effects/NormalMap");
            texture = Content.Load<Texture2D>("Textures/model_diff");
            normalMap = Content.Load<Texture2D>("Textures/model_norm");

            CreateWorld();
        }

        private void CreateWorld()
        {
            world = Matrix.Identity;

            cameraPosition = new Vector3(3, 5, 10);
            lookAt = Vector3.Zero;
            view = Matrix.CreateLookAt(cameraPosition, lookAt, Vector3.Up);

            projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
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

            angle += 0.01f;

            cameraPosition = 10 * new Vector3((float)Math.Sin(angle), 0.5f, (float)Math.Cos(angle));
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            viewVector = lookAt - cameraPosition;
            viewVector.Normalize();

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
                    effect.Parameters["ViewVector"].SetValue(viewVector);
                    effect.Parameters["ModelTexture"].SetValue(texture);
                    effect.Parameters["NormalMap"].SetValue(normalMap);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        #endregion Loop
    }
}
