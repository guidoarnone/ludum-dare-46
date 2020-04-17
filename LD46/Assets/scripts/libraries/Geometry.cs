using UnityEngine;

namespace Geometry {

	public struct Line {
		public Vector3 A;
		public Vector3 B;

		public Line (Vector3 A, Vector3 B) {
			this.A = A;
			this.B = B;
		}
	}

	public struct Triangle {
		public Vector3 A;
		public Vector3 B;
		public Vector3 C;

		public Triangle (Vector3 A, Vector3 B, Vector3 C) {
			this.A = A;
			this.B = B;
			this.C = C;
		}
	}

    public struct Rectangle {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 up;
        public float width;
        public float height;

        public Rectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height) {
            this.position = position;
            this.normal = normal;
            this.up = up;
            this.width = width;
            this.height = height;
        }
    }


    public struct Vector {
		public Vector3 position;
		public Vector3 direction;
		public float magnitude;

		public Vector (Vector3 position, Vector3 direction, float magnitude) {
			this.position = position;
			this.direction = direction;
			this.magnitude = magnitude;
		}
	}

    public struct Circle {
        public Vector3 position;
        public Vector3 normal;
        public float radius;

        public Circle (Vector3 position, Vector3 normal, float radius) {
            this.position = position;
            this.normal = normal;
            this.radius = radius;
        }

        public static Vector2 angle_to_direction(float θ) {
            float x = Mathf.Cos(θ);
            float y = Mathf.Sin(θ);
            return new Vector2 (x, y);
        }
    }

    public struct Sphere {
		public Vector3 position;
		public float radius;

		public Sphere (Vector3 position, float radius) {
			this.position = position;
			this.radius = radius;
		}

        public bool contains(Vector3 point) {
            return Vector3.Distance(point, position) <= radius;
        }

        public Vector3 closest_point(Vector3 point) {
            return (point - position).normalized * radius + position;
        }

        public static Vector3 closest_point(Sphere sphere, Vector3 point) {
            return (point - sphere.position).normalized * sphere.radius + sphere.position;
        }
	}

	public struct Plane {
		public Vector3 position;
		public Vector3 normal;
		public float size;

		public Plane (Vector3 position, Vector3 normal, float size) {
			this.position = position;
			this.normal = normal;
			this.size = size;
		}

        public Vector3[] to_vertices() {
            Vector3[] vertices = new Vector3[4];
            vertices[0] = position + size * Vector3.Cross(normal, Vector3.forward);
            vertices[1] = position + size * Vector3.Cross(normal, Vector3.right);
            vertices[2] = position - size * Vector3.Cross(normal, Vector3.forward);
            vertices[3] = position - size * Vector3.Cross(normal, Vector3.right);
            return vertices;
        }
	}

	public struct Box {
		public Vector3 position;
		public Vector3 size;
		public Vector3 rotation;

		public Box (Vector3 position, Vector3 size, Vector3 rotation) {
			this.position = position;
			this.size = size;
			this.rotation = rotation;
		}
	}

    public struct Cylinder {
        public Vector3 position;
        public float radius;
        public Vector3 height;
        public bool cap;

        public Cylinder(Vector3 position, float radius, Vector3 height, bool cap = true) {
            this.position = position;
            this.radius = radius;
            this.height = height;
            this.cap = cap;
        }
    }

    public struct Prism {
        public Vector3 position;
        public float radius;
        public int sides;
        public Vector3 height;
        public bool cap;

        public Prism(Vector3 position, float radius, int sides, Vector3 height, bool cap = true) {
            this.position = position;
            this.radius = radius;
            this.sides = sides;
            this.height = height;
            this.cap = cap;
        }
    }

    public struct Ribbon {
        public float width;

        public Ribbon(float width) {
            this.width = width;
        }
    }
}