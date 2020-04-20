using UnityEngine;
using UnityEditor;
using System.Collections;
using Geometry;

public static class Draw {
    public static class Gizmo {

        public static void marker(Sphere sphere) {
            float radius = sphere.radius;
            Vector3 position = sphere.position;

            Vector3 A = position + Vector3.up * radius * 0.5f;
            Vector3 B = position - Vector3.up * radius * 0.5f;
            Gizmos.DrawLine(A, B);
            A = position + Vector3.right * radius * 0.5f;
            B = position - Vector3.right * radius * 0.5f;
            Gizmos.DrawLine(A, B);
            A = position + Vector3.forward * radius * 0.5f;
            B = position - Vector3.forward * radius * 0.5f;
            Gizmos.DrawLine(A, B);
        }

        //courtesy of robertbu
        public static void plane(Geometry.Plane plane) {
            Vector3 position = plane.position;
            Vector3 normal = plane.normal;
            float size = plane.size;

            Vector3 V;

            if (normal.normalized != Vector3.forward) {
                V = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
            }
            else { V = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; }

            Vector3 A = position + V * size;
            Vector3 C = position - V * size;

            Quaternion q = Quaternion.AngleAxis(90.0f, normal);
            V = q * V;
            Vector3 B = position + V * size;
            Vector3 D = position - V * size;

            Gizmos.DrawLine(A, C);
            Gizmos.DrawLine(B, D);
            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, C);
            Gizmos.DrawLine(C, D);
            Gizmos.DrawLine(D, A);
            Gizmos.DrawRay(position, normal * size);
        }

        public static void grid(Rectangle rectangle, Int2 divisions) {
            Vector3 position = rectangle.position;
            Vector3 normal = rectangle.normal;
            Vector3 forward = rectangle.forward;
            Vector2 radius = rectangle.size/2;

            Vector3[] vertices = rectangle.to_vertices();

            Gizmos.DrawLine(vertices[0], vertices[1]);
            Gizmos.DrawLine(vertices[1], vertices[2]);
            Gizmos.DrawLine(vertices[2], vertices[3]);
            Gizmos.DrawLine(vertices[3], vertices[0]);
            for (int i = 1; i < divisions.x; i++) {
                float t = (float)i / divisions.x;
                Gizmos.DrawLine(Vector3.Lerp(vertices[0],vertices[3],t), Vector3.Lerp(vertices[1],vertices[2],t));
            }
            for (int i = 1; i < divisions.y; i++) {
                float t = (float)i / divisions.y;
                Gizmos.DrawLine(Vector3.Lerp(vertices[0], vertices[1], t), Vector3.Lerp(vertices[3], vertices[2], t));
            }
            Gizmos.DrawRay(position, normal * radius);
        }

        public static void circle(Geometry.Circle circle, int sides) {
            Vector3 position = circle.position;
            Vector3 normal = circle.normal;
            float radius = circle.radius;

            float step = Math.pi * 2 / sides;
            for (int i = 0; i < sides; i++) {
                float angle = step * i;

                Vector3 X = Vector3.Cross(normal != Vector3.up ? Vector3.up : Vector3.right, normal).normalized;
                Vector3 Y = Vector3.Cross(X, normal).normalized;

                Vector3 A = X * Mathf.Cos(angle) * radius + Y * Mathf.Sin(angle) * radius;
                angle += step;
                Vector3 B = X * Mathf.Cos(angle) * radius + Y * Mathf.Sin(angle) * radius;

                Gizmos.DrawLine(position + A, position + B);
            }
        }

        public static void wheel(Geometry.Circle circle, float wheel_angle, int sides) {
            Vector3 position = circle.position;
            Vector3 normal = circle.normal;
            float radius = circle.radius;

            Vector3 A, B;

            Vector3 X = Vector3.Cross(normal != Vector3.up ? Vector3.up : Vector3.right, normal);
            Vector3 Y = Vector3.Cross(X, normal);

            float step = Math.pi * 2 / sides;
            for (int i = 0; i < sides; i++) {
                float angle = step * i;

                A = X * Mathf.Cos(angle) * radius + Y * Mathf.Sin(angle) * radius;
                angle += step;
                B = X * Mathf.Cos(angle) * radius + Y * Mathf.Sin(angle) * radius;

                Gizmos.DrawLine(position + A, position + B);
            }

            A = X * Mathf.Cos(wheel_angle) * radius + Y * Mathf.Sin(wheel_angle) * radius;
            wheel_angle += Math.pi;
            B = X * Mathf.Cos(wheel_angle) * radius + Y * Mathf.Sin(wheel_angle) * radius;
            Gizmos.DrawLine(position + A, position + B);
            wheel_angle += Math.pi / 2;
            A = X * Mathf.Cos(wheel_angle) * radius + Y * Mathf.Sin(wheel_angle) * radius;
            wheel_angle += Math.pi;
            B = X * Mathf.Cos(wheel_angle) * radius + Y * Mathf.Sin(wheel_angle) * radius;
            Gizmos.DrawLine(position + A, position + B);
        }

        public static void sphere(Sphere sphere, int sides) {
            Vector3 position = sphere.position;
            float radius = sphere.radius;

            float step = Math.pi * 2 / sides;
            for (int i = 0; i < sides; i++) {
                float angle = step * i;
                Vector2 A = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                angle += step;
                Vector2 B = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

                Gizmos.DrawLine(position + new Vector3(0, A.x, A.y), position + new Vector3(0, B.x, B.y));
                Gizmos.DrawLine(position + new Vector3(A.x, 0, A.y), position + new Vector3(B.x, 0, B.y));
                Gizmos.DrawLine(position + new Vector3(A.x, A.y, 0), position + new Vector3(B.x, B.y, 0));

            }
        }

        public static void vector(Vector vector) {
            Vector3 position = vector.position;
            Vector3 direction = vector.direction;
            float magnitude = vector.magnitude;

            Gizmos.DrawRay(position, direction * magnitude);
            Draw.Gizmo.marker(new Sphere(position, 1f));
        }

        public static void triangle(Triangle triangle) {
            Vector3 A = triangle.A;
            Vector3 B = triangle.B;
            Vector3 C = triangle.C;

            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, C);
            Gizmos.DrawLine(C, A);
        }

        public static void triangle(Triangle triangle, Transform T) {
            Vector3 A = triangle.A;
            Vector3 B = triangle.B;
            Vector3 C = triangle.C;
            A = T.TransformPoint(A);
            B = T.TransformPoint(B);
            C = T.TransformPoint(C);

            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, C);
            Gizmos.DrawLine(C, A);
        }

        public static void mesh(Mesh mesh, Transform T) {
            for (int i = 0; i < mesh.triangles.Length; i += 3) {
                Vector3 A = mesh.vertices[mesh.triangles[i]];
                Vector3 B = mesh.vertices[mesh.triangles[i]];
                Vector3 C = mesh.vertices[mesh.triangles[i]];
                triangle(new Triangle(A, B, C), T);
            }
        }
    }

    public static Color random_color() {
        return new Color(Random.value, Random.value, Random.value);
    }
}
