﻿using Entity.Domain.Interfaces;
using Entity.Domain.Models.Base;
using System.Text.RegularExpressions;

namespace Helpers.Business
{
    namespace Business.Helpers.Validation
    {
        /// <summary>
        /// Clase estática que proporciona métodos de ayuda para validaciones comunes en la lógica de negocio.
        /// Todas las validaciones lanzan InvalidOperationException si fallan.
        /// </summary>
        public static class BusinessValidationHelper
        {
            /// <summary>
            /// Lanza una excepción si el objeto es nulo.
            /// </summary>
            /// <param name="obj">Objeto a validar</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando el objeto es nulo</exception>
            public static void ThrowIfNull(object? obj, string message)
            {
                if (obj == null)
                    throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Lanza una excepción si la condición es verdadera.
            /// </summary>
            /// <param name="condition">Condición a evaluar</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando la condición es verdadera</exception>
            public static void ThrowIfTrue(bool condition, string message)
            {
                if (condition)
                    throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Lanza una excepción si la cadena es nula, vacía o solo contiene espacios en blanco.
            /// </summary>
            /// <param name="value">Cadena a validar</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando la cadena es nula o vacía</exception>
            public static void ThrowIfNullOrEmpty(string? value, string message)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Lanza una excepción si el número es negativo.
            /// </summary>
            /// <param name="number">Número a validar</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando el número es negativo</exception>
            public static void ThrowIfNegative(int number, string message)
            {
                if (number < 0)
                    throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Lanza una excepción si el número es cero o menor.
            /// </summary>
            /// <param name="number">Número a validar</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando el número es ≤ 0</exception>
            public static void ThrowIfZeroOrLess(int number, string message)
            {
                if (number <= 0)
                    throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Lanza una excepción si la entidad está marcada como eliminada lógicamente.
            /// </summary>
            /// <param name="entity">Entidad que implementa ISupportLogicalDelete</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando is_deleted es true</exception>
            public static void ThrowIfDeleted(BaseModel entity, string message)
            {
                if (entity.IsDeleted)
                    throw new InvalidOperationException(message);
            }
            public static bool IsValidPassword(string password)
            {
                if (string.IsNullOrWhiteSpace(password)) return false;

                // Al menos 6 caracteres y una mayúscula
                var regex = new Regex(@"^(?=.*[A-Z]).{6,}$");
                return regex.IsMatch(password);
            }

            /// <summary>
            /// Lanza una excepción si la entidad NO está marcada como eliminada lógicamente.
            /// </summary>
            /// <param name="entity">Entidad que implementa ISupportLogicalDelete</param>
            /// <param name="message">Mensaje de error para la excepción</param>
            /// <exception cref="InvalidOperationException">Se lanza cuando IsDeleted es false</exception>
            public static void ThrowIfNotDeleted(BaseModel entity, string message)
            {
                if (!entity.IsDeleted)
                    throw new InvalidOperationException(message);
            }
        }
    }
}