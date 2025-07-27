using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObeserverPattern
{
    public static class EventManager
    {
        private static Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Berlangganan ke sebuah event dengan tipe data tertentu.
        /// </summary>
        /// <param name="listener">Fungsi yang akan dipanggil saat event terjadi.</param>
        /// <typeparam name="T">Tipe data dari event.</typeparam>
        public static void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (eventDictionary.TryGetValue(eventType, out Delegate existingDelegate))
            {
                // Tambahkan listener baru ke delegate yang sudah ada
                eventDictionary[eventType] = Delegate.Combine(existingDelegate, listener);
            }
            else
            {
                // Buat entri baru untuk tipe event ini
                eventDictionary[eventType] = listener;
            }
        }

        /// <summary>
        /// Berhenti berlangganan dari sebuah event.
        /// </summary>
        /// <param name="listener">Fungsi yang ingin dihapus.</param>
        /// <typeparam name="T">Tipe data dari event.</typeparam>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (eventDictionary.TryGetValue(eventType, out Delegate existingDelegate))
            {
                Delegate resultDelegate = Delegate.Remove(existingDelegate, listener);
                if (resultDelegate == null)
                {
                    // Hapus entri dari dictionary jika tidak ada listener lagi
                    eventDictionary.Remove(eventType);
                }
                else
                {
                    eventDictionary[eventType] = resultDelegate;
                }
            }
        }

        /// <summary>
        /// Menerbitkan atau "menyiarkan" sebuah event ke semua pelanggannya.
        /// </summary>
        /// <param name="eventData">Data yang akan dikirim bersama event.</param>
        /// <typeparam name="T">Tipe data dari event.</typeparam>
        public static void Raise<T>(T eventData)
        {
            Type eventType = typeof(T);
            if (eventDictionary.TryGetValue(eventType, out Delegate existingDelegate))
            {
                // Panggil semua listener yang terdaftar untuk event ini
                (existingDelegate as Action<T>)?.Invoke(eventData);
            }
        }
    }
}
