using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Test
{
    public class TexturePaint : MonoBehaviour
    {
        [SerializeField] private string _seed;
        [SerializeField] private int _quality;

        //[SerializeField] private GeoConfiguration _configuration;

        private Texture2D _texture;
        private Image _image;
        private int _width;
        private int _height;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                //Geography.SetSeed(_seed);
                _width = 1920 / _quality;
                _height = 1080 / _quality;
                _texture = new Texture2D(_width, _height);
                _image.sprite = Sprite.Create(_texture, new Rect(0, 0, _width, _height), Vector2.one / 2f);
                TextureGenerate();
                Debug.Log($"Карта сгенерировалась. Кол-во пикселей: {_width * _height}");
            }
        }

        private void TextureGenerate()
        {
            var pixels = new Color32[_width * _height];

            Parallel.For((long)0, _height, y =>
            {
                for (var x = 0; x < _width; x++)
                {
                    var index = y * _width + x;
                    pixels[index] = GetColor(new Vector2Int(x, (int)y));
                }
            });
            _texture.SetPixelData(pixels, 0);

            /* Медленный способ
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _texture.SetPixel(x, y, _settings.GetColor(new Vector2Int(x, y)));
                }
            }*/

            _texture.filterMode = FilterMode.Point;
            _texture.Apply();
        }

        private Color32 GetColor(Vector2Int axial)
        {
            Color32 color = new Color32(255, 255, 255, 255);

            /*var elevation = _configuration.GetElevation(axial);
            var isLand = _configuration.LandMinElevation <= elevation;
            var humidity = _configuration.GetHumidity(axial);
            var temperature = _configuration.GetTemperature(axial);

            if (isLand)
                color = new Color32(0, 155, 0, 255);
            else color = new Color32(28, 169, 201, 255);
            */

            return color;
        }
    }
}