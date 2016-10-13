using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho;
using Urho.Gui;
using Urho.Holographics;
using Urho.Navigation;
using Urho.Physics;
using Urho.Portable.Holographics;

namespace ElBruno.Urho02
{
    public class PositionTextSample : HoloApplication
    {
        private Node _positionNodeDebug;
        private Node _positionSelectorNodeText;
        private Node _environmentNode;
        private Material _spatialMaterial;
        private Vector3 _envPositionBeforeManipulations;
        private Text3D _text3DDebug;
        private readonly Queue<Node> _textQueue = new Queue<Node>();

        private bool _debugMode;
        private const int MaxTexts = 5;
        private int _textCount;

        public PositionTextSample(string pak, bool emulator) : base(pak, emulator) { }

        protected override async void Start()
        {
            base.Start();
            _environmentNode = Scene.CreateChild();
            Scene.CreateComponent<SpatialCursor>();
            
            // Allow tap gesture
            EnableGestureTapped = true;

            await RegisterCortanaCommands(new Dictionary<string, Action>
            {
                //play animations using Cortana
                {"clean", () => ClearAll()},
                {"debug", () => EnableDisableDebugMode()},
                {"set text" , () => PositionText()}
            });


            // Material for spatial surfaces
            _spatialMaterial = new Material();
            _spatialMaterial.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);

            //var allowed = await StartSpatialMapping(new Vector3(50, 50, 10), 1200);
            var spatialMappingAllowed = await StartSpatialMapping(new Vector3(50, 50, 10), 1200, onlyAdd: true);
        }

        private void CreateDebugInformationNode()
        {
            if(_positionNodeDebug != null) return;
            _positionNodeDebug = Scene.CreateChild();
            _positionNodeDebug.Scale = new Vector3(0.3f, 0.15f, 0.2f);
            _text3DDebug = _positionNodeDebug.CreateComponent<Text3D>();
            _text3DDebug.HorizontalAlignment = HorizontalAlignment.Center;
            _text3DDebug.VerticalAlignment = VerticalAlignment.Top;
            _text3DDebug.ViewMask = 0x80000000; //hide from raycasts
            _text3DDebug.Text = $@"@Debug {DateTime.Now:T}";
            _text3DDebug.SetFont(CoreAssets.Fonts.AnonymousPro, 10);
            _text3DDebug.SetColor(Color.White);
        }

        private void EnableDisableDebugMode()
        {
            _debugMode = !_debugMode;
        }

        private void ClearAll()
        {
            while (_textQueue.Count > 0)
            {
                _textQueue.Dequeue().Remove();
            }
        }

        Vector3? Raycast()
        {
            Ray cameraRay = LeftCamera.GetScreenRay(0.5f, 0.5f);
            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);
            if (result != null)
            {
                return result.Value.Position;
            }
            return null;
        }

        Vector3? RayCastDebug()
        {
            var previewPosition = LeftCamera.Node.Position + (LeftCamera.Node.Direction * 1.5f);
            return previewPosition;
        }

        Vector3? RayCastText()
        {
            var hitPos = Raycast();
            if (hitPos != null)
            {
                hitPos = hitPos.Value - (LeftCamera.Node.Direction * 0.2f);
            }
            return hitPos;
        }

        protected override void OnUpdate(float timeStep)
        {
            if (!_debugMode)
            {
                Scene.RemoveChild(_positionNodeDebug);
            }
            else
            {
                var hitPos = RayCastDebug();
                if (hitPos == null)
                    return;
                CreateDebugInformationNode();
                _positionNodeDebug.Position = hitPos.Value;
                _positionNodeDebug.LookAt(LeftCamera.Node.WorldPosition, new Vector3(0, 1, 0), TransformSpace.World);
                _positionNodeDebug.Rotate(new Quaternion(0, 180, 0), TransformSpace.World);
                _text3DDebug.Text = $@"Debug {timeStep.ToString()}";

            }

        }

        public override void OnGestureTapped(GazeInfo gaze)
        {
            PositionText();
            base.OnGestureTapped(gaze);
        }

        private void PositionText()
        {
            var hitPos = RayCastText();
            if (hitPos == null)
                return;

            _textCount++;
            _positionSelectorNodeText = Scene.CreateChild();
            _positionSelectorNodeText.Scale = new Vector3(0.3f, 0.15f, 0.2f);
            _positionSelectorNodeText.Position = hitPos.Value;
            _positionSelectorNodeText.LookAt(LeftCamera.Node.WorldPosition, new Vector3(0, 1, 0), TransformSpace.World);
            _positionSelectorNodeText.Rotate(new Quaternion(0, 180, 0), TransformSpace.World);

            var text3D = _positionSelectorNodeText.CreateComponent<Text3D>();
            text3D.HorizontalAlignment = HorizontalAlignment.Center;
            text3D.VerticalAlignment = VerticalAlignment.Top;
            text3D.ViewMask = 0x80000000; //hide from raycasts
            text3D.Text = $@"@ElBruno {_textCount}
{DateTime.Now:T}";
            text3D.SetFont(CoreAssets.Fonts.AnonymousPro, 26);
            text3D.SetColor(Color.White);

            _textQueue.Enqueue(_positionSelectorNodeText);
            if (_textQueue.Count > MaxTexts)
                _textQueue.Dequeue().Remove();


        }

        public override void OnSurfaceAddedOrUpdated(SpatialMeshInfo surface, Model generatedModel)
        {
            bool isNew;
            StaticModel staticModel;
            Node node = _environmentNode.GetChild(surface.SurfaceId, false);
            if (node != null)
            {
                isNew = false;
                staticModel = node.GetComponent<StaticModel>();
            }
            else
            {
                isNew = true;
                node = _environmentNode.CreateChild(surface.SurfaceId);
                staticModel = node.CreateComponent<StaticModel>();
            }

            node.Position = surface.BoundsCenter;
            node.Rotation = surface.BoundsRotation;
            staticModel.Model = generatedModel;

            if (isNew)
            {
                staticModel.SetMaterial(_spatialMaterial);
                var rigidBody = node.CreateComponent<RigidBody>();
                rigidBody.RollingFriction = 0.5f;
                rigidBody.Friction = 0.5f;
                var collisionShape = node.CreateComponent<CollisionShape>();
                collisionShape.SetTriangleMesh(generatedModel, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);
            }
            else
            {
                //Update Collision shape
            }
        }
        public override void OnGestureManipulationStarted()
        {
            _envPositionBeforeManipulations = _environmentNode.Position;
        }
        public override void OnGestureManipulationUpdated(Vector3 relativeHandPosition)
        {
            _environmentNode.Position = relativeHandPosition + _envPositionBeforeManipulations;
        }

    }
}